using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class DoorSieveMapping : ISieveMapping<Door>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Door>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<Door>(p => p.DoorName)
                .CanSort()
                .CanFilter();
            mapper.Property<Door>(p => p.DoorArea)
                .CanSort()
                .CanFilter();
            mapper.Property<Door>(p => p.LoadingType)
                .CanSort()
                .CanFilter();
            mapper.Property<Door>(p => p.Sequence)
                .CanSort()
                .CanFilter();
            
        }
    }
}
