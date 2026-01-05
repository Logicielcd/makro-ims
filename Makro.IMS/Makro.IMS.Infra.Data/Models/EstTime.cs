using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class EstTime
{
    public int InternalEstId { get; set; }

    public int? InternalSupGroupId { get; set; }

    public int? InternalTruckId { get; set; }

    public int? HourEst { get; set; }

    public int? MinEst { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? WarehouseCode { get; set; }

    public string? OperationType { get; set; }
}
