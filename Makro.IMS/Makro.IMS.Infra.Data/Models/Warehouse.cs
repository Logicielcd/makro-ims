using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Warehouse
{
    public string WarehouseCode { get; set; } = null!;

    public string? CompanyCode { get; set; }

    public string? WarehouseName { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Zipcode { get; set; }

    public string? ContactName { get; set; }

    public string? BookingIdPrefix { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? ContactEMail { get; set; }

    public string? MobileNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public int? BookingIdRunning { get; set; }

    public int? FirstTimeOfDay { get; set; }

    public int? EndTimeOfDay { get; set; }

    public int? TimeWidth { get; set; }

    public decimal? TimeIncreaseStep { get; set; }

    public bool? Active { get; set; }

    public string? Note { get; set; }

    public int? MaxConPerHour { get; set; }

    public int? MaxNonPerHour { get; set; }

    public int? MaxFullPerHour { get; set; }

    public int? MaxAllPerHour { get; set; }

    public decimal? MaxCConPerHour { get; set; }

    public decimal? MaxCNonPerHour { get; set; }

    public decimal? MaxCFullPerHour { get; set; }

    public decimal? MaxCAllPerHour { get; set; }

    public string? OnlineBookingIdPrefix { get; set; }

    public decimal? OnlineBookingIdRunning { get; set; }

    public string? WarehouseMain { get; set; }

    public DateTime? DateUpdateRunning { get; set; }

    public string? FixDoor { get; set; }

    public string? WarehouseLevel { get; set; }

    public int? AdvanceBookingPeriod { get; set; }

    public int? PoBeforePeriod { get; set; }

    public int? PoAfterPeriod { get; set; }

    public int? AdvanceCheckinTime { get; set; }

    public int? LateCheckinTime { get; set; }

    public string? CapacityType { get; set; }

    public string? WarehouseWms { get; set; }

    public string? SendCallTruckSms { get; set; }

    public string? SendReceiveDocSms { get; set; }

    public string? CapUom { get; set; }

    public int? AdvanceBookingDay { get; set; }
}
