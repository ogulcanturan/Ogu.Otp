using System.Collections.Generic;

namespace Ogu.Otp
{
    public class HotpTokenProvider : BaseOtpTokenProvider
    {
        public HotpTokenProvider(string issuer, HashAlgorithmKind hashAlgorithmKind, DigitCount digitCount, ushort pastTolerance, ushort futureTolerance) : base(issuer, hashAlgorithmKind, digitCount, pastTolerance, futureTolerance) { }

        public string GetUri(string audience, string secretKey, long counter = 0)
        {
            return GetUri(
                audience,
                secretKey,
                "hotp",
                new KeyValuePair<string, string>("counter", counter.ToString()));
        }

        public override string GenerateCode(string secretKey, long counter, string modifier = null)
        {
            return base.GenerateCode(secretKey, counter, modifier);
        }

        public override OtpValidationResult ValidateCode(string code, string secret, long counter, string modifier = null)
        {
            return base.ValidateCode(code, secret, counter, modifier);
        }
    }
}