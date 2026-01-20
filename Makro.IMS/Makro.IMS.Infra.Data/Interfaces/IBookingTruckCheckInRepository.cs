using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingTruckCheckInRepository : IDisposable
    {
        IEnumerable<BookingTruckCheckIn> GetBookingCheckInByHeaderId(int bookingHdrId);
        BookingTruckCheckIn GetBookingCheckInById(int id);
        bool Add(BookingTruckCheckIn bookingCheckIn);
        bool Remove(BookingTruckCheckIn bookingCheckIn);
        bool Update(BookingTruckCheckIn bookingCheckIn);

        List<BookingTruckCheckIn> GetLastQueueId(DateTime assignQueuDate);
        
    }

}
