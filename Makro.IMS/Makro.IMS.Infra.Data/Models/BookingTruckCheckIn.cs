using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingTruckCheckIn
{
    public decimal? InternalHeaderKey { get; set; }

    public decimal? InternalTruckId { get; set; }

    public string? LicensePlate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public string? UserStamp { get; set; }

    public string? DriverName { get; set; }

    public string? TelNo { get; set; }

    public decimal InternalTruckCheckInId { get; set; }

    public string? LineId { get; set; }

    public decimal? QueueSeq { get; set; }

    public DateTime? ArrivedTime { get; set; }

    public DateTime? SubmitdocTime { get; set; }

    public DateTime? OndockTime { get; set; }

    public DateTime? StartUnloadTime { get; set; }

    public DateTime? FinishUnloadTime { get; set; }

    public DateTime? DepartureTime { get; set; }

    public DateTime? AssignQueueTime { get; set; }

    public string? Status { get; set; }

    public DateTime? CalltruckTime { get; set; }

    public DateTime? CheckoutTime { get; set; }

    public DateTime? DoccheckTime { get; set; }

    public string? WaitingDocument { get; set; }

    public string? Remark { get; set; }

    public DateTime? WaitingDocumentTime { get; set; }

    public DateTime? DateTimeStamp { get; set; }

    public string? Door { get; set; }

    public string? LicensePlate2 { get; set; }

    public string? YardIn { get; set; }

    public string? YardOut { get; set; }
}
