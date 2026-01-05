using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Shunt
{
    public decimal Id { get; set; }

    public string? LicensePlate { get; set; }

    public string? DriverName { get; set; }

    public string? TelNo { get; set; }

    public string? Status { get; set; }

    public decimal? LocationId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }
}
