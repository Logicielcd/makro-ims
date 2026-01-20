using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class PoCheckInOut
{
    public string BookingId { get; set; } = null!;

    public decimal InternalTruckCheckinId { get; set; }

    public decimal Id { get; set; }

    public string PoNbr { get; set; } = null!;

    public DateTime? DateTimeStamp { get; set; }

    public string? UserStamp { get; set; }

    public string? Status { get; set; }
}
