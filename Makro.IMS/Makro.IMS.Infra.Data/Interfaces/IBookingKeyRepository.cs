using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingKeyRepository : IDisposable
    {
        IEnumerable<BookingKey> GetBookingKeys();    
        BookingKey? GetBookingKeyById(int keyId);
        IEnumerable<BookingKey>? GetBookingKeyByBookingDate(DateTime bookingDate);

        bool Add(BookingKey bookingKey);
        bool Remove(BookingKey bookingKey);
        bool Update(BookingKey bookingKey);
        
    }

}
