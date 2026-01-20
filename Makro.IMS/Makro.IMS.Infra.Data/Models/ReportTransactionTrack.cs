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
    public class ReportTransactionTrack
    {
        public string WarehouseCode { get; set; }
        public string CompanyCode { get; set; }
        public int InternalTruckCheckInId { get; set; }
        public string? BookingId { get; set; }
        public string? LicensePlate { get; set; }
        public string? LicensePlate2 { get; set; }
        public string? DriverName { get; set; }
        public string? TruckCode { get; set; }
        public string? SupCode { get; set; }
        public string? SupName { get; set; }
        public string? MerchType { get; set; }
        public string? Door { get; set; }
        public int? TotalPoBooking { get; set; }
        public string? PoBooking { get; set; }
        public int? TotalPoChecking { get; set; }
        public string? PoChecking { get; set; }
        public int? TotalPoConfirm { get; set; }
        public string? PoConfirm { get; set; }
        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? ArrivedTime { get; set; }
        public int? QWaiting { get; set; }
        public DateTime? AssignQueueTime { get; set; }
        public int? CallWaiting { get; set; }
        public DateTime? CallTruckTime { get; set; }
        public int? OnDockWaiting { get; set; }
        public DateTime? OnDockTime { get; set; }
        public int? UnloadWaiting { get; set; }
        public DateTime? StartUnloadTime { get; set; }
        public int? UnloadedWaiting { get; set; }
        public DateTime? FinishUnloadTime { get; set; }
        public int? LeaveWaiting { get; set; }
        public DateTime? DepartureTime { get; set; }
        public int? DocWaiting { get; set; }
        public DateTime? WaitingDocumentTime { get; set; }
        public int? SubmitWaiting { get; set; }
        public DateTime? SubmitDocTime { get; set; }
        public int? CheckoutWaiting { get; set; }
        public DateTime? CheckoutTime { get; set; }

        public int? TotalTime { get; set; }
        public string? Remark { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalTruck { get; set; }
        public string? UserId { get; set; }
        public string? UserType { get; set; }
        public string? BackHaul { get; set; }
        public string? RemarkBooking { get; set; }
        public string? RemarkDelay { get; set; }
    }

}
