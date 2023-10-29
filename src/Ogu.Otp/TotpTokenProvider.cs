using System;
using System.Collections.Generic;

namespace Ogu.Otp
{
    public class TotpTokenProvider : BaseOtpTokenProvider, ITotpTokenProvider
    {
        public readonly TimeSpan Period;
        public readonly TimeSpan TimeDifference;
   
        public TotpTokenProvider(string issuer, TimeSpan? period = null, HashAlgorithmKind hashAlgorithmKind = HashAlgorithmKind.Sha1, DigitCount digitCount = DigitCount.Six,  ushort pastTolerance = 0, ushort futureTolerance = 0, TimeSpan? timeDifference = null) : base(issuer, hashAlgorithmKind, digitCount, pastTolerance, futureTolerance)
        {
            Period = period ?? TimeSpan.FromSeconds(30);
            TimeDifference = timeDifference ?? TimeSpan.Zero;
        }

        public string GetUri(string audience, string secretKey)
        {
            return base.GetUri(
                audience,
                secretKey,
                "totp",
                new KeyValuePair<string, string>("period", ((long)Period.TotalSeconds).ToString()));
        }

        public string GenerateCode(string secretKey, string modifier = null)
        {
            var timestep = GetCurrentTimestepNumber(Period, TimeDifference);

            return base.GenerateCode(secretKey, timestep, modifier);
        }

        public OtpValidationResult ValidateCode(string code, string secretKey, string modifier = null)
        {
            var timestep = GetCurrentTimestepNumber(Period, TimeDifference);

            return base.ValidateCode(code, secretKey, timestep, modifier);
        }

#if NETSTANDARD2_0 || NETFRAMEWORK
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#endif
        private static long GetCurrentTimestepNumber(TimeSpan period, TimeSpan timeDifference)
        {
#if NETSTANDARD2_0 || NETFRAMEWORK
            var delta = DateTime.UtcNow.Add(timeDifference) - _unixEpoch;
#else
            var delta = DateTime.UtcNow.Add(timeDifference) - DateTimeOffset.UnixEpoch;
#endif
            return delta.Ticks / period.Ticks;
        }
    }
}