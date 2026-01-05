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
    public class PoList
    {
        public string? Internal_Po_No { get; set; }
        public string? Company_Code { get; set; }
        public string? Warehouse_Code { get; set; }
        public string? Po_Nbr { get; set; }
        public string? Sup_Code { get; set; }
        public DateTime Plan_Receive_Date { get; set; }
        public DateTime Create_Date { get; set; }
        public int? Booker { get; set; }
        public string? Remark { get; set; }
        public string? Booking_Id { get; set; }
        public int? Total_Qty { get; set; }
        public int? Full { get; set; }
        public int? Con { get; set; }
        public int? Non { get; set; }
        public decimal? Cube_Full { get; set; }
        public decimal? Cube_Con { get; set; }
        public decimal? Cube_Non { get; set; }
        public string PostPoned { get; set; }
        public string? Merch_Type { get; set; }
        public decimal? Weight { get; set; }
        public string? Warehouse_Name { get; set; }
        public string? Company_Name { get; set; }
        public long? Full_Cs { get; set; }
        public long? Half { get; set; }
        public long? Half_Cs { get; set; }
        public DateTime? Expire_Date { get; set; }
        public bool? Is_Delay { get; set; }
        public string? Delay_Reason { get; set; }
    }

    public class BookingCheckInDto
    {
        public int InternalHeaderKey { get; set; }
        public int InternalDetailKey { get; set; }
        public string PoNbr { get; set; }
        public string WarehouseCode { get; set; }
        public string BookingId { get; set; }
        public string TruckType { get; set; }
        public string DriverName { get; set; }
        public string LicensePlate { get; set; }
        public string TelNo { get; set; }
        public string LineId { get; set; }
        public string? MerchType { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class QueueManageDto
    {
        public int InternalHeaderKey { get; set; }
        public int InternalTruckCheckInId { get;set; }
        public string BookingId { get; set; }
        public string WarehouseCode { get; set; }
        public string CompanyCode { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public string OperationType { get; set; }
        public DateTime BookingStart { get; set; }
        public DateTime BookingEnd { get; set; }
        public string LicensePlate { get; set; }
        public string? LicensePlate2 { get; set; }
        public string Truck { get; set; }
        public string TelNo { get; set; }
        public string? QueueSeq { get; set; }
        public DateTime? ArrivedTime { get; set; }
        public DateTime? OnDockTime { get; set; }
        public int WaitingTime { get; set; }
        public string? Status { get; set; }
        public string? WaitingDocument { get; set; }
        public string? Remark { get; set; }
        public DateTime? WaitingDocumentTime { get; set; }
        public int TotalTruck { get; set; }
        public DateTime? LastUpDate { get; set; }
        public string? BackHaul { get; set; }
    }

    public class DoorQueueDto
    {
        public int InternalDoorId { get; set; }
        public string DoorName { get; set; }
        public string? DoorArea { get; set; }
        public string? TruckType { get; set; }
        public int? BookingHeaderKey { get; set; }
        public string? BookingId { get; set; }
        public string? SupCode { get; set; }
        public string? SupName { get; set; }
        public DateTime? ArrivedTime { get; set; }
        public DateTime? SubmitDocTime { get; set; }
        public DateTime? OnDockTime { get; set; }
        public DateTime? StartUnloadTime { get;  set; }
        public DateTime? FinishUnloadTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? CalltruckTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public DateTime? DoccheckTime { get; set; }
        public string? LicensePlate { get; set; }
        public string? LicensePlate2 { get; set; }
        public string? BookingTruckType { get; set; }
        public int? InternalTruckCheckInId { get; set; }
        public int? WaitingTime { get; set; }
    }

    public class TruckCheckIn
    {
        public string BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string WarehouseCode { get; set; }
        public string CompanyCode { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public string OperationType { get; set; }
        public DateTime BookingStart { get; set; }
        public DateTime BookingEnd { get; set; }
        public string LicensePlate { get; set; }
        public string TruckType { get; set; }
        public string DriverName { get; set; }
        public string TelNo { get; set; }
        
    }

}
