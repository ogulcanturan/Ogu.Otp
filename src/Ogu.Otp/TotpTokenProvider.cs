using System;
using System.Collections.Generic;

namespace Ogu.Otp
{
    public class TotpTokenProvider : BaseOtpTokenProvider
    {
        private readonly TimeSpan _period;
        private readonly TimeSpan _timeDifference;
   
        public TotpTokenProvider(string issuer, HashAlgorithmKind hashAlgorithmKind, DigitCount digitCount, TimeSpan period, ushort pastTolerance, ushort futureTolerance, TimeSpan? timeDifference = null) : base(issuer, hashAlgorithmKind, digitCount, pastTolerance, futureTolerance)
        {
            _period = period;
            _timeDifference = timeDifference ?? TimeSpan.Zero;
        }

        public string GetUri(string audience, string secretKey)
        {
            return GetUri(
                audience,
                secretKey,
                "totp",
                new KeyValuePair<string, string>("period", ((long)_period.TotalSeconds).ToString()));
        }

        public string GenerateCode(string secretKey, string modifier = null)
        {
            var timestep = GetCurrentTimestepNumber(_period, _timeDifference);

            return base.GenerateCode(secretKey, timestep, modifier);
        }

        public OtpValidationResult ValidateCode(string code, string secret, string modifier = null)
        {
            var timestep = GetCurrentTimestepNumber(_period, _timeDifference);

            return base.ValidateCode(code, secret, timestep, modifier);
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