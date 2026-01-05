using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Frontend.Api.Dto
{
    public class GuardCheckInOutDto
    {
        public int InternalHeaderKey { get; set; }
        public int InternalTruckCheckInId { get; set; }
        public string BookingId { get; set; }
        public string UserName { get; set; }


    }
}
