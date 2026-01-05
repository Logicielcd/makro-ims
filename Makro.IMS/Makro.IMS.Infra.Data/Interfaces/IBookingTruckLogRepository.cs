using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingTruckLogRepository : IDisposable
    {
        IEnumerable<BookingTruckLog> GetBookingTruckLogByInternalTruckCheckinId(int internalTruckCheckinId);
        BookingTruckLog GetBookingTruckLogById(int id);
        bool Add(BookingTruckLog bookingTruckLog);
        bool Remove(BookingTruckLog bookingTruckLog);
        bool Update(BookingTruckLog bookingTruckLog);

        
    }

}
