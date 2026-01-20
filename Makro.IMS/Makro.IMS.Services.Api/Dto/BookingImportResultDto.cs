using System.DirectoryServices.Protocols;

namespace Makro.IMS.Services.Api.Dto
{
    public class BookingImportResultDto
    {
        public string WarehouseCode { get; set; }
        public string OperationType { get; set; }
        public DateTime TimeSlot { get; set; }
        public string BookingId { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalTruck { get; set; }
    }
}
