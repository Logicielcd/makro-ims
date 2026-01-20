using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Services.Api.Dto
{
    public class InboundBookingDto
    {
        public string WarehouseCode { get; set; }
        public string OperationType { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public int SubGroupId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LicensePlate { get; set; }
        public string LicensePlate2 { get; set; }
        public string DriverName { get; set; }
        public string TelNo { get; set; }
        public string TruckType { get; set; }
        public string UserName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string LineNo { get; set; }


    }
}
