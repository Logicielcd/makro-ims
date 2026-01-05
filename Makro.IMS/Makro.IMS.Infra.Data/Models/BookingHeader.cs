using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingHeader
{
    public int InternalHeaderKey { get; set; }

    public int? InternalKeyId { get; set; }

    public int? InternalDoorId { get; set; }

    public int? InternalSupGroupId { get; set; }

    public string? WarehouseCode { get; set; }

    public string? SupCode { get; set; }

    public string? SupName { get; set; }

    public DateTime? BookingStart { get; set; }

    public DateTime? BookingEnd { get; set; }

    public int? TotalPo { get; set; }

    public int? TotalQty { get; set; }

    public string? UserStamp { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public bool? Postponed { get; set; }

    public bool? BackHaul { get; set; }

    public bool? Active { get; set; }

    public DateTime? FirstBookginStart { get; set; }

    public DateTime? FirstBookingEnd { get; set; }

    public string? FirstUserStamp { get; set; }

    public string? BookingId { get; set; }

    public string? RevisionPrefix { get; set; }

    public int? RevisionRunning { get; set; }

    public string? Remark { get; set; }

    public DateTime? OrgNeedDate { get; set; }

    public string? Status { get; set; }

    public string? ContactName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTel { get; set; }

    public string? ApproveCondition { get; set; }

    public string? MerchType { get; set; }

    public string? CompanyCode { get; set; }

    public string? OriginalMerchType { get; set; }

    public bool? IsDelay { get; set; }

    public string? RemarkDelay { get; set; }

    public string? RemarkCancel { get; set; }

    public string? UserCancel { get; set; }
}
