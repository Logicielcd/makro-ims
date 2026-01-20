using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingTruckLog
{
    public decimal Id { get; set; }

    public decimal InternalTruckCheckInId { get; set; }

    public string Action { get; set; } = null!;

    public string? Remark { get; set; }

    public string UserStamp { get; set; } = null!;

    public DateTime DateTimeStamp { get; set; }
}
