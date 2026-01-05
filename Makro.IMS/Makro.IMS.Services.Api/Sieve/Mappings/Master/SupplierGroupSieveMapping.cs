using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class SupplierGroupSieveMapping : ISieveMapping<SupplierGroup>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<SupplierGroup>(p => p.SupName)
                .CanSort()
                .CanFilter();
            mapper.Property<SupplierGroup>(p => p.ContactName)
                .CanSort()
                .CanFilter();
            mapper.Property<SupplierGroup>(p => p.ContactEMail)
                .CanSort()
                .CanFilter();
            mapper.Property<SupplierGroup>(p => p.PhoneNumber)
                .CanSort()
                .CanFilter();
            mapper.Property<SupplierGroup>(p => p.MobileNumber)
                .CanSort()
                .CanFilter();
            mapper.Property<SupplierGroup>(p => p.InternalSupGroupId)
                .CanSort()
                .CanFilter();
        }
    }
}
