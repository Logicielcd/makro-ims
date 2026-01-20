using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class OperationTime
{
    public decimal Id { get; set; }

    public string OperationType { get; set; } = null!;

    public string WarehouseCode { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}
