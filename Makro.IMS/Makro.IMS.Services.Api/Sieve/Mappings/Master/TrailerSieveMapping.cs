using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class TrailerSieveMapping : ISieveMapping<Trailer>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Trailer>(p => p.LicensePlate)
                .CanSort()
                .CanFilter();
            mapper.Property<Trailer>(p => p.TrailerType)
                .CanSort()
                .CanFilter();
            mapper.Property<Trailer>(p => p.TrailerGroup)
               .CanSort()
               .CanFilter();
            mapper.Property<Trailer>(p => p.TrailerSize)
               .CanSort()
               .CanFilter();
            mapper.Property<Trailer>(p => p.LocationId)
                .CanSort()
                .CanFilter();
            mapper.Property<Trailer>(p => p.Status)
                .CanSort()
                .CanFilter();
            mapper.Property<Trailer>(p => p.UserStamp)
                .CanSort()
                .CanFilter();
            mapper.Property<Trailer>(p => p.ModDate)
               .CanSort()
               .CanFilter();
            mapper.Property<Trailer>(p => p.CreateDate)
                .CanSort()
                .CanFilter();
        }
    }
}
