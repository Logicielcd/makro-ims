using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IBookingDetailRepository : IDisposable
    {
        IEnumerable<BookingDetail> GetBookingDetailsByHeaderId(int bookingHdrId);
        BookingDetail? GetBookingDetailById(int detailId);

        BookingDetail GetBookingDetailByPoNo(string poNo);

        BookingDetail GetBookingDetailByPoNoAndId(string poNo,int bookingHdrId);
        bool Add(BookingDetail bookingDetail);
        bool Remove(BookingDetail bookingDetail);
        bool Update(BookingDetail bookingDetail);


    }

}
