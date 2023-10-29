using Ogu.Otp;
using System;

namespace Otp.Api.Services
{
    public class EmailTotpToken : TotpTokenProvider // Sample
    {
        public EmailTotpToken(string issuer, TimeSpan period) : base(issuer, period) { }
    }
}