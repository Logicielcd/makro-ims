using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingTruck
{
    public int InternalHeaderKey { get; set; }

    public int InternalTruckId { get; set; }

    public int? TotalTruck { get; set; }

    public string? Remark { get; set; }
}
