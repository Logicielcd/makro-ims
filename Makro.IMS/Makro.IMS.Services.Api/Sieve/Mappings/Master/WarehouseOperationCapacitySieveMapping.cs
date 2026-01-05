using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class WarehouseOperationCapacitySieveMapping : ISieveMapping<WarehouseOperationCapacity>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<WarehouseOperationCapacity>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.OperationType)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.BookingDate)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.MaxConPerHour)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.MaxNonPerHour)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.CreateDate)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.UserStamp)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseOperationCapacity>(p => p.ModDate)
                .CanSort()
                .CanFilter();
        }
    }
}
