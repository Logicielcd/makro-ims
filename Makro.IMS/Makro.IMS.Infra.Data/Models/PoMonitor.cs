using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Makro.IMS.Infra.Data.Models;

public partial class PoMonitor
{
    public string WarehouseCode { get; set; }
    public string CompanyCode { get; set; }
    public string SupCode { get; set; }
    public string SupName { get; set; }
    public string? BookingId { get; set; }
    public DateTime? BookingStart { get; set; }
    public DateTime? BookingEnd { get; set; }
    public DateTime? BookingCreateDate { get; set; }
    public string? UserCreate { get; set; }
    public string PoNbr { get; set; }
    public DateTime? PoCreateDate { get; set; }
    public DateTime? PlanReceivedDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? RemarkDelay { get; set; }
    public string? RemarkBooking { get; set; }
    public string? Comment1 { get; set; }
    public string? Comment2 { get; set; }
    public string? Comment3 { get; set; }
    public string? Comment4 { get; set; }
    public string? Comment5 { get; set; }
    public string? Comment6 { get; set; }
}
