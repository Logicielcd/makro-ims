using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingDetail
{
    public int InternalDetailKey { get; set; }

    public int? InternalHeaderKey { get; set; }

    public string? PoNbr { get; set; }

    public int? TotalQty { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? Remark { get; set; }

    public DateTime? PlanRec { get; set; }

    public decimal? FullPl { get; set; }

    public decimal? Con { get; set; }

    public decimal? Non { get; set; }

    public decimal? CubeFull { get; set; }

    public decimal? CubeCon { get; set; }

    public decimal? CubeNon { get; set; }

    public string? Status { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public DateTime? PreCheckIn { get; set; }

    public string? Postponed { get; set; }

    public string? MerchType { get; set; }

    public DateTime? DocumentCheckIn { get; set; }

    public decimal? Weight { get; set; }

    public long? HalfPl { get; set; }

    public decimal? FullCs { get; set; }

    public decimal? HalfCs { get; set; }

    public string? DelayReason { get; set; }

    public bool? IsDelay { get; set; }

    public DateTime? ExpireDate { get; set; }
}
