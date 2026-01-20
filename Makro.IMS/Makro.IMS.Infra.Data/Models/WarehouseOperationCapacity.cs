using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class WarehouseOperationCapacity
{
    public decimal Id { get; set; }

    public string WarehouseCode { get; set; } = null!;

    public string? OperationType { get; set; }

    public DateTime BookingDate { get; set; }

    public int? MaxConPerHour { get; set; }

    public int? MaxNonPerHour { get; set; }

    public int? MaxFullPerHour { get; set; }

    public int? MaxAllPerHour { get; set; }

    public decimal? MaxCConPerHour { get; set; }

    public decimal? MaxCNonPerHour { get; set; }

    public decimal? MaxCFullPerHour { get; set; }

    public decimal? MaxCAllPerHour { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }
}
