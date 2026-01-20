using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class SupplierSieveMapping : ISieveMapping<Supplier>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<Supplier>(p => p.SupCode)
                .CanSort()
                .CanFilter();
            mapper.Property<Supplier>(p => p.SupName)
                .CanSort()
                .CanFilter();
            mapper.Property<Supplier>(p => p.InternalGroupId)
                .CanSort()
                .CanFilter();
            //mapper.Property<Supplier>(p => p.InternalSupGroupId)
            //    .CanSort()
            //    .CanFilter();
        }
    }
}
