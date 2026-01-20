using Microsoft.EntityFrameworkCore;
using Makro.IMS.Infra.Data.Context;

namespace Makro.IMS.POService.Api.Configurations
{
    public static class DatabaseConfig
    {

        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<IMSContext>(options => options.UseOracle(configuration.GetConnectionString("DefaultConnection")
                , b => b.UseOracleSQLCompatibility("11")
                ));            
            
        }
    }
}