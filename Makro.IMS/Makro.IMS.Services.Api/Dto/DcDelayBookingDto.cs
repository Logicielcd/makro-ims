using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Dto
{
    public class DcDelayBookingDto
    {
        public int BookingHeaderId { get; set; }
        public List<BookingDetail> BookingDetails { get; set; }
    }

}
