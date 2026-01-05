using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class OperationFixSlot
{
    public decimal Id { get; set; }

    public string WarehouseCode { get; set; } = null!;

    public string OperationType { get; set; } = null!;

    public int SupGroupId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? IsVip { get; set; }

    public string DaysOfWeek { get; set; } = null!;

    public virtual SupplierGroup SupGroup { get; set; } = null!;
}
