using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class WarehouseCapacitySieveMapping : ISieveMapping<WarehouseCapacity>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<WarehouseCapacity>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.CompanyCode)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.BookingDate)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.MaxConPerHour)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.MaxNonPerHour)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.CreateDate)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.UserStamp)
                .CanSort()
                .CanFilter();
            mapper.Property<WarehouseCapacity>(p => p.ModDate)
                .CanSort()
                .CanFilter();
        }
    }
}
