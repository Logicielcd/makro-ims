using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class OperationCapacity
{
    public decimal Id { get; set; }

    public string? WarehouseCode { get; set; }

    public string? OperationType { get; set; }

    public DateTime? Time { get; set; }

    public decimal? MonCap { get; set; }

    public decimal? TueCap { get; set; }

    public decimal? WedCap { get; set; }

    public decimal? ThuCap { get; set; }

    public decimal? FriCap { get; set; }

    public decimal? SatCap { get; set; }

    public decimal? SunCap { get; set; }

    public decimal? MonTruck { get; set; }

    public decimal? TueTruck { get; set; }

    public decimal? WedTruck { get; set; }

    public decimal? ThuTruck { get; set; }

    public decimal? FriTruck { get; set; }

    public decimal? SatTruck { get; set; }

    public decimal? SunTruck { get; set; }
}
