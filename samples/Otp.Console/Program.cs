using Ogu.Otp;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Otp.Console
{
    public class Program
    {
        private static readonly TotpTokenProvider _totp = new TotpTokenProvider(
            "Ogu-Totp",
            TimeSpan.FromSeconds(30),
            HashAlgorithmKind.Sha1,
            DigitCount.Six,
            2,
            2);

        private static readonly HotpTokenProvider _hotp = new HotpTokenProvider(
            "Ogu-Hotp",
            HashAlgorithmKind.Sha1,
            DigitCount.Six,
            3,
            3);

        public static void Main(string[] args)
        {
            while (true)
            {
                System.Console.WriteLine("Press 1 for Generating TOTP code\n" +
                                         "Press 2 for Validation TOTP code\n" +
                                         "Press 3 for Generating HOTP code\n" +
                                         "Press 4 for Validation HOTP code\n");

                _ = int.TryParse(System.Console.ReadLine(), out var value);

                switch (value)
                {
                    case 1:
                        GenerateTotpCode();
                        break;
                    case 2:
                        ValidateTotpCode();
                        break;
                    case 3:
                        GenerateHotpCode();
                        break;
                    case 4:
                        ValidateHotpCode();
                        break;
                }
            }
        }

        private static Dictionary<int, string> _totpGeneratedSecretKeys = new();
        private static Dictionary<int, (string, long)> _hotpGeneratedSecretKeys = new();

        public static void GenerateTotpCode()
        {
            var secretKey = _totp.GenerateSecretKey();
            var code = _totp.GenerateCode(secretKey);
            _totpGeneratedSecretKeys.Add(_totpGeneratedSecretKeys.Count, secretKey);
            var uri = _totp.GetUri("John", secretKey);

            var otpValidationResult = _totp.ValidateCode(code, secretKey);

            var builder = new StringBuilder()
                .Append("SecretKey: ").Append(secretKey).AppendLine()
                .Append("Code: ").Append(code).AppendLine()
                .Append("Uri: ").Append(uri).AppendLine()
                .Append("IsCodeValid: ").Append(otpValidationResult.IsValid).AppendLine();

            System.Console.WriteLine(builder.ToString());

            GenerateQrCode(uri, false);
        }

        public static void ValidateTotpCode()
        {
            if (_totpGeneratedSecretKeys.Count < 1)
            {
                System.Console.WriteLine("To validate first generate Totp secret key!");
                return;
            }

            string secretKey;

            if (_totpGeneratedSecretKeys.Count != 1)
            {
                System.Console.WriteLine("*** Found more than one keys ***");

                var builder = new StringBuilder();

                foreach (var item in _totpGeneratedSecretKeys.Select((x, i) => new { SecretKey = x.Value, Index = i }))
                {
                    builder.Append("[").Append(item.Index).Append("]: ").Append(item.SecretKey).Append('\n');
                }

                System.Console.WriteLine(builder.ToString());

                System.Console.WriteLine("Enter the index of available secure keys");

                _ = int.TryParse(System.Console.ReadLine(), out var index);

                bool canGetValue = _totpGeneratedSecretKeys.TryGetValue(index, out secretKey);

                if (!canGetValue)
                {
                    System.Console.WriteLine("You typed wrong index!");
                    return;
                }
            }
            else
            {
                secretKey = _totpGeneratedSecretKeys[0];
            }

            System.Console.WriteLine("Enter the code:");

            var code = System.Console.ReadLine();

            var totpValidationResult = _totp.ValidateCode(code, secretKey);

            System.Console.WriteLine($"IsCodeValid: {totpValidationResult.IsValid}\n" +
                                     $"Tolerance: {totpValidationResult.Tolerance}");
        }

        public static void GenerateHotpCode()
        {
            var secretKey = _hotp.GenerateSecretKey();
            var code = _hotp.GenerateCode(secretKey);
            _hotpGeneratedSecretKeys.Add(_hotpGeneratedSecretKeys.Count, (secretKey, 0));
            var uri = _hotp.GetUri("John", secretKey);
            
            var otpValidationResult = _hotp.ValidateCode(code, secretKey, 0);

            var builder = new StringBuilder()
                .Append("SecretKey: ").Append(secretKey).AppendLine()
                .Append("Code: ").Append(code).AppendLine()
                .Append("Uri: ").Append(uri).AppendLine()
                .Append("IsCodeValid: ").Append(otpValidationResult.IsValid).AppendLine();

            System.Console.WriteLine(builder.ToString());

            GenerateQrCode(uri, false);
        }

        public static void ValidateHotpCode()
        {
            if (_hotpGeneratedSecretKeys.Count < 1)
            {
                System.Console.WriteLine("To validate first generate Hotp secret key!");
                return;
            }

            string secretKey;
            long counter;

            if (_hotpGeneratedSecretKeys.Count != 1)
            {
                System.Console.WriteLine("*** Found more than one keys ***");

                var builder = new StringBuilder();

                foreach (var item in _hotpGeneratedSecretKeys.Select((x, i) => new { SecretKey = x.Value.Item1, Index = i }))
                {
                    builder.Append("[").Append(item.Index).Append("]: ").Append(item.SecretKey).Append('\n');
                }

                System.Console.WriteLine(builder.ToString());

                System.Console.WriteLine("Enter the index of available secure keys");

                _ = int.TryParse(System.Console.ReadLine(), out var index);

                bool canGetValue = _hotpGeneratedSecretKeys.TryGetValue(index, out var result);

                if (!canGetValue)
                {
                    System.Console.WriteLine("You typed wrong index!");
                    return;
                }

                secretKey = result.Item1;
                counter = result.Item2;
                _hotpGeneratedSecretKeys[index] = new(secretKey, counter+1);
            }
            else
            {
                secretKey = _hotpGeneratedSecretKeys[0].Item1;
                counter = _hotpGeneratedSecretKeys[0].Item2;
                _hotpGeneratedSecretKeys[0] = new(secretKey, counter+1);
            }

            System.Console.WriteLine("Enter the code:");

            var code = System.Console.ReadLine();

            var hotpValidationResult = _hotp.ValidateCode(code, secretKey, counter);

            System.Console.WriteLine($"IsCodeValid: {hotpValidationResult.IsValid}\n" +
                                     $"Tolerance: {hotpValidationResult.Tolerance}\n" +
                                     $"CurrenctCounter: {counter}\n" +
                                     $"NextCounter: {++counter}");
        }

        // Package: QRCoder
        private static void GenerateQrCode(string uri, bool saveAsPng)
        {
            var qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.M);

            var asciiQrCode = new AsciiQRCode(qrCodeData);
            var qrCodeString = asciiQrCode.GetGraphic(1);

            System.Console.WriteLine(qrCodeString);

            if (saveAsPng) // Only supports windows platform - Use QRCoder-ImageSharp if targeting other platforms as well.
            {
                PngByteQRCode sd = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = sd.GetGraphic(20);

                Bitmap qrCodeImage;
                using (var stream = new MemoryStream(qrCodeBytes))
                {
                    qrCodeImage = new Bitmap(stream);
                }

                // Save QR code as PNG file
                string filePath = $"{Guid.NewGuid()}.png";

                qrCodeImage.Save(filePath, ImageFormat.Png);
            }
        }
    }
}