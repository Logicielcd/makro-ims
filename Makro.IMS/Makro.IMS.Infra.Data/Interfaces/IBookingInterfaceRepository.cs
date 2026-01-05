using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingInterfaceRepository : IDisposable
    {
        IEnumerable<BookingInterface> GetBookingInterfaceByBookingId(string bookingId);
        BookingInterface GetBookingInterfaceById(int id);
        bool Add(BookingInterface poLog);
        bool Remove(BookingInterface poLog);
        bool Update(BookingInterface poLog);

        
    }

}
