using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class PoMonitorSieveMapping : ISieveMapping<PoMonitor>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<PoMonitor>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.CompanyCode)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.SupCode)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.SupName)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.BookingId)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.BookingStart)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.BookingEnd)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.BookingCreateDate)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.UserCreate)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.PoNbr)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.PoCreateDate)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.PlanReceivedDate)
                .CanSort()
                .CanFilter();
            mapper.Property<PoMonitor>(p => p.ExpireDate)
                .CanSort()
                .CanFilter();


        }
    }
}
