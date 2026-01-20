using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class WarehouseSieveMapping : ISieveMapping<Warehouse>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Warehouse>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<Warehouse>(p => p.CompanyCode)
                .CanSort()
                .CanFilter();
            mapper.Property<Warehouse>(p => p.WarehouseName)
                .CanSort()
                .CanFilter();
            mapper.Property<Warehouse>(p => p.WarehouseLevel)
                .CanSort()
                .CanFilter();
            mapper.Property<Warehouse>(p => p.WarehouseMain)
                .CanSort()
                .CanFilter();
            mapper.Property<Warehouse>(p => p.FixDoor)
                .CanSort()
                .CanFilter();
        }
    }
}
