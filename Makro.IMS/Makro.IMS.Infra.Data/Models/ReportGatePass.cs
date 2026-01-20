using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Models
{
    public class ReportGatePass
    {
        public string? BookingId { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseWms { get; set; }
        public string? CompanyCode { get; set; }
        public string? WarehouseName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? PoNbr { get; set; }
        public string? SupCode { get; set; }
        public string? SupName { get; set; }
        public string? ContactTel { get; set; }
        public DateTime BookingStart { get; set; }
        public string? LicensePlate { get; set; }
        public string? DriverName { get; set; }
        public string? TelNo { get; set; }
        public string? TruckCode { get; set; }
        public decimal? TotalQty { get; set; }
        public string? MerchType { get; set; }
       
        public DateTime WarehouseArrived { get; set; }
        public string? OriginalMerchType { get; set; }
        public decimal? FullPl { get; set; }
        public decimal? HalfPl { get; set; }
        public decimal? LooseQty { get; set; }
        public DateTime? OrderDate { get; set; }
        public string IsLate { get; set; }
        public string? LicensePlate2 { get; set; }
        public int Id { get; set; }
        public int No { get; set; }
        public string? Reason { get; set; }
        public bool BackHaul { get; set; }
    }

}
