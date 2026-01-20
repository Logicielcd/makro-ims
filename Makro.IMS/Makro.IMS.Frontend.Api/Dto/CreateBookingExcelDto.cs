using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Frontend.Api.Dto
{
    public class CreateBookingExcelDto
    {
        public string SupplierCode { get; set; }
        public int InternalSupGroupId { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime BookingDate { get; set; }        
        public List<CreateBookingExcelWarehouse> CreateBookingExcelWarehouses { get; set; }
        public List<BookingHeader>? BookingHeaders { get; set; }

    }

    public class CreateBookingExcelWarehouse
    {
        public string WarehouseCode { get; set; }
        public string PoNo { get; set; }
        public string TruckType { get; set; }
        public string TruckNo { get; set; }
        public string TruckGroup { get; set; }
        public string Remark { get; set; }
        public string BookingGroup { get; set; }
        public DateTime? TimeSlot { get; set; }
        public string ImportResult { get; set; }


    }



}
