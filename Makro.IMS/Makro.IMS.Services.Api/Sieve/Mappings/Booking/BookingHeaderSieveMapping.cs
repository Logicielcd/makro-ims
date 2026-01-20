using Sieve.Services;
using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Sieve.Mappings.Master
{
    public class BookingHeaderSieveMapping : ISieveMapping<BookingHeader>
    {
        public void ConfigureMap(SievePropertyMapper mapper)
        {
            mapper.Property<BookingHeader>(p => p.WarehouseCode)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.BookingId)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.SupCode)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.SupName)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.BookingStart)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.BookingEnd)
                .CanSort()
                .CanFilter();            
            mapper.Property<BookingHeader>(p => p.TotalPo)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.TotalQty)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.Active)
                .CanSort()
                .CanFilter();
            mapper.Property<BookingHeader>(p => p.Postponed)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.Status)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.MerchType)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.CompanyCode)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.InternalKeyId)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.ApproveCondition)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.CreateDate)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.IsDelay)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.BackHaul)
                .CanSort()
                .CanFilter();

            mapper.Property<BookingHeader>(p => p.RevisionPrefix)
                .CanSort()
                .CanFilter();
        }
    }
}
