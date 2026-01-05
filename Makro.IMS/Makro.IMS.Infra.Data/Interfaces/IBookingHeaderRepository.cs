using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingHeaderRepository : IDisposable
    {
        IEnumerable<BookingHeader> GetBookingHeaders();
        IQueryable<BookingHeader> GetBookingHeadersPaged();
        IEnumerable<BookingHeader>? GetDashboard(string warehouseCode);
        BookingHeader? GetBookingHeaderById(int keyId);
        IEnumerable<BookingHeader>? GetBookingHeaderBySupplierAndBookingDate(string supCode, DateTime bookingDate);
        IEnumerable<BookingHeader>? GetBookingHeaderByKeyId(int keyId);

        BookingHeader? GetBookingHeaderByDoorAndSlotTime(int internalDoorId,DateTime startDate,DateTime endDate);
        BookingHeader GetBookingHeadersByBookingId(string bookingId);

        IEnumerable<BookingHeader>? GetBookingHeaderForCheckIn(string warehouseCode);

        bool Add(BookingHeader bookingHeader);
        bool Remove(BookingHeader bookingHeader);
        bool Update(BookingHeader bookingHeader);

        bool UpdateStatus(int id,string status);
    }

}
