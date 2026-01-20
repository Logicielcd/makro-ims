using Sieve.Services;
using Makro.IMS.Infra.Data.Models;
using System.Reflection.Emit;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class OperationSieveMapping : ISieveMapping<Operation>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Operation>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<Operation>(p => p.OperationName)
                .CanSort()
                .CanFilter();
            mapper.Property<Operation>(p => p.Description)
                .CanSort()
                .CanFilter();
        }
    }
}
