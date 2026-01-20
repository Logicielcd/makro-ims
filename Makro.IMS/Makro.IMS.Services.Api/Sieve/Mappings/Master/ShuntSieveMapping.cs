using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class ShuntSieveMapping : ISieveMapping<Shunt>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Shunt>(p => p.LicensePlate)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.DriverName)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.TelNo)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.Status)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.LocationId)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.CreateDate)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.ModDate)
                .CanSort()
                .CanFilter();
            mapper.Property<Shunt>(p => p.UserStamp)
                .CanSort()
                .CanFilter();
        }
    }
}
