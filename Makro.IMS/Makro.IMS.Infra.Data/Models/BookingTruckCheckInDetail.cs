using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingTruckCheckInDetail
{
    public decimal? InternalTruckCheckInId { get; set; }

    public decimal InternalTruckDetailId { get; set; }

    public DateTime? CheckInTime { get; set; }

    public string? PoNbr { get; set; }

    public decimal? InternalDetailKey { get; set; }
}
