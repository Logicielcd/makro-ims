using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;
using Makro.IMS.POServices.Api.Sieve;

namespace Makro.IMS.POServices.Api.Configurations
{
    public static class SievePagingConfiguration
    {
        public static void AddSieveConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ISieveCustomFilterMethods, SieveCustomFilterMethods>();
            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();


            services.Configure<SieveOptions>(config.GetSection("Sieve"));
        }
    }
}
