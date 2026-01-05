using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Services.Api.Dto
{
    public class BookingKeyDto
    {
        public int? InternalKeyId { get; set; }
        public DateTime BookingDate { get; set; }
        public List<BookingHeaderDto> BookingHeaders { get; set; }
        public string UserName { get; set; }

    }

    public class BookingHeaderDto
    {
        public int? InternalHeaderKey { get; set; }
        public int InternalDoorId { get; set; }
        public int InternalSupGroupId { get; set; }
        public string WarehouseCode { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public string? DockDoor { get; set; }
        public DateTime BookingStart { get; set; }
        public DateTime BookingEnd { get; set; }
        // public int TotalPo { get; set; }
        // public int TotalQty { get; set; }
        public DateTime FirstBookingStart { get; set; }
        public DateTime FirstBookingEnd { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTel { get; set; }
        public List<BookingDetail> BookingDetails { get; set; }
        public List<BookingTruck> BookingTrucks { get; set; }
        public List<BookingTruckCheckInDto>? BookingCheckIns { get; set; }
        public string? BookingId { get; set; }
        public string? TotalTruck { get; set; }
        public bool PostPoned { get; set; }
        public string? Status { get; set; }
        public string? MerchType { get; set; }
        public string? CompanyCode { get; set; }
        public bool BackHaul { get; set; }
        public string? RemarkDelay { get; set; }
        public string? FirstUserStamp { get; set; }
        public DateTime? ModDate { get; set; }
        public string? UserStamp { get; set; }
        public string? Remark { get; set; }
        public string? RemarkCancel { get; set; }
    }

    public class BookingTruckCheckInDto
    {
        public int InternalHeaderKey { get; set; }
        public int InternalTruckId { get; set; }
        public int? InternalTruckCheckInId { get; set; }
        public string DriverName { get; set; }
        public string LicensePlate { get; set; }
        public string? LicensePlate2 { get; set; }
        public string TelNo { get; set; }
        public string PoNo { get; set; }
        public string UserStamp { get; set; }
        public string LineId { get; set; }
        public string? TruckType { get; set; }
        public decimal? QueueSeq { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? ArrivedTime { get; set; }

        public DateTime? SubmitdocTime { get; set; }

        public DateTime? OndockTime { get; set; }

        public DateTime? StartUnloadTime { get; set; }

        public DateTime? FinishUnloadTime { get; set; }

        public DateTime? DepartureTime { get; set; }

        public DateTime? AssignQueueTime { get; set; }
        public DateTime? CalltruckTime { get; set; }
        public DateTime? DoccheckTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public string? Status { get; set; }
        public DateTime? DateTimeStamp { get; set; }
        public List<BookingTruckCheckInDetail>? BookingTruckCheckInDetails { get; set; }
    }

    public class BookingCheckOutDto
    {
        public string PoNo { get; set; }
        public string SupCode { get; set; }
        public string UserStamp { get; set; }
    }

    public class PreCheckIn
    {
        public string WarehouseCode { get; set; }
        public int InternalHeaderKey { get; set; }

        public List<BookingTruckCheckInDto>? BookingTruckCheckIns { get; set; }
    }

    public class CheckInDto
    {
        public string PoNbr { get; set; }
        public string SupCode { get; set; }
        public string UserStamp { get; set; }
        public string? BookingId { get; set; }
        public int? BookingHeaderId { get; set; }
        public int? InternalTruckCheckInId { get; set; }
        public string? Remark { get; set; }
    }

}
