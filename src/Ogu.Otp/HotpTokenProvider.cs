using System.Collections.Generic;

namespace Ogu.Otp
{
    public class HotpTokenProvider : BaseOtpTokenProvider, IHotpTokenProvider
    {
        public HotpTokenProvider(string issuer, HashAlgorithmKind hashAlgorithmKind = HashAlgorithmKind.Sha1, DigitCount digitCount = DigitCount.Six, ushort pastTolerance = 0, ushort futureTolerance = 0) : base(issuer, hashAlgorithmKind, digitCount, pastTolerance, futureTolerance) { }

        public string GetUri(string audience, string secretKey, long counter = 0)
        {
            return base.GetUri(
                audience,
                secretKey,
                "hotp",
                new KeyValuePair<string, string>("counter", counter.ToString()));
        }

        public new string GenerateCode(string secretKey, long counter = 0, string modifier = null)
        {
            return base.GenerateCode(secretKey, counter, modifier);
        }

        public new OtpValidationResult ValidateCode(string code, string secretKey, long counter, string modifier = null)
        {
            return base.ValidateCode(code, secretKey, counter, modifier);
        }
    }
}