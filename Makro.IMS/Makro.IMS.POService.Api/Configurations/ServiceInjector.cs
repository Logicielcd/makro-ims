using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.POServices.Api.Services;

namespace Makro.IMS.POServices.Api.Configurations
{
    public static class ServiceInjector
    {

        public static void RegisterServices(this IServiceCollection services)
        {
            // User Master            
            services.AddScoped<PoService>();         
        }

    }
}
