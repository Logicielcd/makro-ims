using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class QueueSequence
{
    public string WarehouseCode { get; set; } = null!;

    public string OperationType { get; set; } = null!;

    public decimal LastSequence { get; set; }

    public DateTime? QueueDate { get; set; }
}
