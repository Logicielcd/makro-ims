using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class TruckRule
{
    public string InternalTruckId { get; set; } = null!;

    public string Warehouse { get; set; } = null!;

    public string? OperationType { get; set; }

    public string? QtyType { get; set; }
}
