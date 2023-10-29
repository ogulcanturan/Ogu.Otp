using System;
using System.Net;
using System.Security.Cryptography;

namespace Ogu.Otp
{
    public class Rfc6238Helper
    {
        public static int ComputeOtp(
#if NET6_0_OR_GREATER
        byte[] key,
#else
            HashAlgorithm hashAlgorithm,
#endif
            long variant,
            sbyte digitCount,
            byte[] modifierBytes)
        {

#if NET6_0_OR_GREATER
            Span<byte> timestepAsBytes = stackalloc byte[sizeof(long)];
            _ = BitConverter.TryWriteBytes(timestepAsBytes, IPAddress.HostToNetworkOrder(variant));
#else
            var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(variant));
#endif

#if NET6_0_OR_GREATER
            Span<byte> modifierCombinedBytes = timestepAsBytes;
            if (modifierBytes != null)
            {
                modifierCombinedBytes = ApplyModifier(timestepAsBytes, modifierBytes);
            }
            Span<byte> hash = stackalloc byte[key.Length];

            switch ((HashAlgorithmKind)key.Length)
            {
                case HashAlgorithmKind.Md5:
                    _ = HMACMD5.TryHashData(key, modifierCombinedBytes, hash, out var _);
                    break;
                case HashAlgorithmKind.Sha1:
                    _ = HMACSHA1.TryHashData(key, modifierCombinedBytes, hash, out var _);
                    break;
                case HashAlgorithmKind.Sha256:
                    _ = HMACSHA256.TryHashData(key, modifierCombinedBytes, hash, out var _);
                    break;
                case HashAlgorithmKind.Sha384:
                    _ = HMACSHA384.TryHashData(key, modifierCombinedBytes, hash, out var _);
                    break;
                case HashAlgorithmKind.Sha512:
                    _ = HMACSHA512.TryHashData(key, modifierCombinedBytes, hash, out var _);
                    break;
            }
#else
            var hash = hashAlgorithm.ComputeHash(modifierBytes != null
                ? ApplyModifier(timestepAsBytes, modifierBytes)
                : timestepAsBytes);
#endif
            var offset = hash[hash.Length - 1] & 0xf;
            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | hash[offset + 3] & 0xff;

            int mod = (int)Math.Pow(10, digitCount);

            return binaryCode % mod;
        }

        private static byte[] ApplyModifier(Span<byte> input, byte[] modifierBytes)
        {
            var combined = new byte[checked(input.Length + modifierBytes.Length)];
            input.CopyTo(combined);
            Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
            return combined;
        }
    }
}