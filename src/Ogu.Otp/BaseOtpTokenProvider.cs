using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Ogu.Otp
{
    public abstract class BaseOtpTokenProvider
    {
        public readonly string Issuer;
        public readonly HashAlgorithmKind HashAlgorithmKind;
        public readonly DigitCount DigitCount;
        public readonly int PastTolerance;
        public readonly int FutureTolerance;

        protected BaseOtpTokenProvider(string issuer, HashAlgorithmKind hashAlgorithmKind = HashAlgorithmKind.Sha1, DigitCount digitCount = DigitCount.Six, ushort pastTolerance = 0, ushort futureTolerance = 0)
        {
            Issuer = issuer;
            HashAlgorithmKind = hashAlgorithmKind;
            DigitCount = digitCount;
            PastTolerance = -pastTolerance;
            FutureTolerance = futureTolerance;

            if (PastTolerance > FutureTolerance)
            {
                throw new InvalidOperationException("PastTolerance cannot be greater than FutureTolerance");
            }
        }

        protected string GetUri(string audience, string secret, string otpType, KeyValuePair<string, string> pair)
        {
            var builder = new StringBuilder("otpauth://")
                .Append(otpType)
                .Append("/")
                .Append(Uri.EscapeDataString(Issuer))
                .Append(":")
                .Append(Uri.EscapeDataString(audience))
                .Append("?");

            var parameters = new Dictionary<string, string>(4)
            {
                { "secret", secret.TrimEnd('=') },
                { "algorithm", HashAlgorithmKind.ToString().ToUpper() },
                { "digits", $"{(byte)DigitCount}" },
                { pair.Key, pair.Value }
            };

            foreach (var param in parameters)
            {
                builder
                    .Append(param.Key)
                    .Append("=")
                    .Append(param.Value)
                    .Append("&");
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        public string GenerateSecretKey() => Base32Helper.GenerateBase32(HashAlgorithmKind);

        protected string GenerateCode(string secretKey, long variant, string modifier = null)
        {
            int otp;

            var keyBytes = Base32Helper.FromBase32(secretKey);

            var modifierBytes = string.IsNullOrWhiteSpace(modifier) ? null : Encoding.UTF8.GetBytes(modifier);

#if NET6_0_OR_GREATER
            otp = Rfc6238Helper.ComputeOtp(keyBytes, variant, (sbyte)DigitCount, modifierBytes);
#else
            otp = GetExpectedCode(HashAlgorithmKind, (sbyte)DigitCount, variant, keyBytes, 0, modifierBytes);
#endif
            return otp.ToString($"D{(sbyte)DigitCount}", CultureInfo.InvariantCulture);
        }

        protected OtpValidationResult ValidateCode(string code, string secret, long variant, string modifier = null)
        {
            if (code.Length != (sbyte)DigitCount)
            {
                return new OtpValidationResult(false);
            }

            var keyBytes = Base32Helper.FromBase32(secret);

            var modifierBytes = string.IsNullOrWhiteSpace(modifier) ? null : Encoding.UTF8.GetBytes(modifier);

            int expectedCode;

            for (int i = PastTolerance; i <= FutureTolerance; i++)
            {
#if NET6_0_OR_GREATER
                expectedCode = Rfc6238Helper.ComputeOtp(keyBytes, (variant + i), (sbyte)DigitCount, modifierBytes);
#else
                expectedCode = GetExpectedCode(HashAlgorithmKind, (sbyte)DigitCount, variant, keyBytes, i, modifierBytes);
#endif
                if (expectedCode.ToString($"D{(sbyte)DigitCount}", CultureInfo.InvariantCulture) == code)
                {
                    return i != 0 ?
                        new OtpValidationResult(true, i) :
                        new OtpValidationResult(true);
                }
            }

            return new OtpValidationResult(false);
        }

#if !NET6_0_OR_GREATER
        private static int GetExpectedCode(HashAlgorithmKind hashAlgorithmKind, sbyte digitCount, long variant, byte[] keyBytes, int i, byte[] modifierBytes)
        {
            int expectedCode;
            switch (hashAlgorithmKind)
            {
                case HashAlgorithmKind.Md5:

                    using (var hash = new HMACMD5(keyBytes))
                    {
                        expectedCode = Rfc6238Helper.ComputeOtp(hash, (variant + i), digitCount, modifierBytes);
                    }

                    break;

                case HashAlgorithmKind.Sha1:

                    using (var hash = new HMACSHA1(keyBytes))
                    {
                        expectedCode = Rfc6238Helper.ComputeOtp(hash, (variant + i), digitCount, modifierBytes);
                    }

                    break;

                case HashAlgorithmKind.Sha256:

                    using (var hash = new HMACSHA256(keyBytes))
                    {
                        expectedCode = Rfc6238Helper.ComputeOtp(hash, (variant + i), digitCount, modifierBytes);
                    }

                    break;

                case HashAlgorithmKind.Sha384:

                    using (var hash = new HMACSHA384(keyBytes))
                    {
                        expectedCode = Rfc6238Helper.ComputeOtp(hash, (variant + i), digitCount, modifierBytes);
                    }

                    break;

                case HashAlgorithmKind.Sha512:

                    using (var hash = new HMACSHA512(keyBytes))
                    {
                        expectedCode = Rfc6238Helper.ComputeOtp(hash, (variant + i), digitCount, modifierBytes);
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }

            return expectedCode;
        }
#endif
    }
}