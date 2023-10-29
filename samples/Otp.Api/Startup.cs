using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ogu.Otp;
using Otp.Api.Services;
using System;

namespace Otp.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new EmailTotpToken("Otp Api", TimeSpan.FromMinutes(10))); // Sample
            services.AddSingleton(new TotpTokenProvider("Otp Api - Totp", pastTolerance: 1));
            services.AddSingleton(new HotpTokenProvider("Otp Api - Hotp"));
            services.AddSingleton<TotpTokenFactory>(); // Sample
            services.AddControllers();
            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new OpenApiInfo() { Title = "Otp.Web", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Otp.Web");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}