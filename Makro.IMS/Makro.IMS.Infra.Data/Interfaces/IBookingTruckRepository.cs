using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingTruckRepository : IDisposable
    {
        IEnumerable<BookingTruck> GetBookingTrucksByHeaderId(int bookingHdrId);
        BookingTruck? GetBookingTruckById(int bookingHdrId, int truckId);


        bool Add(BookingTruck bookingTruck);
        bool Remove(BookingTruck bookingTruck);
        bool Update(BookingTruck bookingTruck);
    }

}
