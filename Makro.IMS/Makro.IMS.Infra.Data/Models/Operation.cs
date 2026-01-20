using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Operation
{
    public string OperationName { get; set; } = null!;

    public string WarehouseCode { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Capacity { get; set; }

    public decimal? CapacityLarge { get; set; }

    public bool? Overcap { get; set; }

    public bool? Overcutoff { get; set; }
}
