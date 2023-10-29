using Microsoft.Extensions.DependencyInjection;
using Ogu.Otp;
using Otp.Api.Enums;
using System;

namespace Otp.Api.Services
{
    public class TotpTokenFactory // Sample
    {
        private readonly IServiceProvider _serviceProvider;

        public TotpTokenFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITotpTokenProvider GetProvider(TotpTokenType totpTokenType)
        {
            switch (totpTokenType)
            {
                case TotpTokenType.MultiFactor:
                    return _serviceProvider.GetRequiredService<TotpTokenProvider>();
                case TotpTokenType.Email:
                    return _serviceProvider.GetRequiredService<EmailTotpToken>();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}