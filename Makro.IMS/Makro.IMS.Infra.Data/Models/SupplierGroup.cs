using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class SupplierGroup
{
    public int InternalSupGroupId { get; set; }

    public string? SupName { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Zipcode { get; set; }

    public string? MobileNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ContactName { get; set; }

    public string? ContactEMail { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? CompanyCode { get; set; }

    public string? UserDef1 { get; set; }

    public string? UserDef2 { get; set; }

    public string? UserDef3 { get; set; }

    public string? UserDef4 { get; set; }

    public string? UserDef5 { get; set; }

    public string? UserDef6 { get; set; }

    public string? PkSupCode { get; set; }

    public bool? FixTime { get; set; }

    public int? InternalSupTypeId { get; set; }

    public bool? ComfirmGatePass { get; set; }

    public string? Remark { get; set; }

    public DateTime? RemarkCreateDate { get; set; }

    public string? RemarkToSup { get; set; }

    public string? Remark2 { get; set; }

    public string? SizeType { get; set; }

    public string? Postpond { get; set; }

    public string? FixTimeStart { get; set; }

    public string? FixTimeEnd { get; set; }

    public string? IsUpCreateBook { get; set; }

    public string? IsUpPreCheckin { get; set; }

    public string? IsVip { get; set; }

    public decimal? MaxBookingPreHour { get; set; }

    public DateTime? InterfaceDate { get; set; }

    public string? Warehouses { get; set; }

    public string? BuyerCode { get; set; }

    public string? RmsCode { get; set; }

    public string? WarehouseCutoff { get; set; }

    public virtual ICollection<OperationFixSlot> OperationFixSlots { get; set; } = new List<OperationFixSlot>();
}
