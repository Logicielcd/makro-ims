using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class TruckCap
{
    public decimal? InternalTruckId { get; set; }

    public string? WarehouseCode { get; set; }

    public decimal? FullPl { get; set; }

    public string OperationType { get; set; } = null!;
}
