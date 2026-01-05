using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingTruckCheckInDetailRepository : IDisposable
    {
        IEnumerable<BookingTruckCheckInDetail> GetByCheckInId(int checkInId);

        IEnumerable<BookingTruckCheckInDetail> GetByPo(int checkInId, string poNbr);
        bool Add(BookingTruckCheckInDetail bookingCheckInDetail);
        bool Remove(BookingTruckCheckInDetail bookingCheckInDetail);
        bool Update(BookingTruckCheckInDetail bookingCheckInDetail);
    }

}
