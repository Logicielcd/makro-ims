using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class YardSieveMapping : ISieveMapping<Yard>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Yard>(p => p.YardNo)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.YardType)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.YardZone)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.TrailerType)
               .CanSort()
               .CanFilter();
            mapper.Property<Yard>(p => p.LocationNo)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.Status)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.UserStamp)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.CreateDate)
                .CanSort()
                .CanFilter();
            mapper.Property<Yard>(p => p.ModDate)
                .CanSort()
                .CanFilter();
        }
    }
}
