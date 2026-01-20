using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingInterface
{
    public decimal Id { get; set; }

    public string? BookingId { get; set; }

    public string? Status { get; set; }

    public bool? IsInterface { get; set; }

    public DateTime? DateTimeStamp { get; set; }

    public string? UserStamp { get; set; }
}
