using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;
using Makro.IMS.Services.Api.Sieve;

namespace Makro.IMS.Services.Api.Configurations
{
    public static class SievePagingConfiguration
    {
        public static void AddSieveConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ISieveCustomFilterMethods, SieveCustomFilterMethods>();
            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
            services.AddScoped<ISieveIListProcessor, ApplicationSieveIListProcessor>();

            services.Configure<SieveOptions>(config.GetSection("Sieve"));
        }
    }
}
