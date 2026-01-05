using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class OperationTimeSieveMapping : ISieveMapping<OperationTime>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<OperationTime>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<OperationTime>(p => p.OperationType)
                .CanSort()
                .CanFilter();            
        }
    }
}
