using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Services.Api.Dto
{
    public class PreCheckInExcelDto
    {
        public List<PreCheckInExcelDetailDto> PreCheckInExcelDetailDtos { get; set; }
    }
    public class PreCheckInExcelDetailDto
    {
        public string BookingId { get; set; }
        public string WarehouseCode { get; set; }
       // public string PoNo { get; set; }
        public string TruckType { get; set; }
        public string TruckLicense { get; set; }
        public string? TruckLicense2 { get; set; }
        public string TruckSequence { get; set; }
        public string DriverName { get; set; }
        public string TelNo { get; set; }
        public string? LineNo { get; set; }
        public string SupCode { get; set; }
        public int InternalSupGroupId { get; set; }
        public string ImportResult { get; set; }
    }


}
