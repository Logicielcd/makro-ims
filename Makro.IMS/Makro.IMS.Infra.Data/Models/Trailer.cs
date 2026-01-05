using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Trailer
{
    public decimal Id { get; set; }

    public string? LicensePlate { get; set; }

    public string? TrailerType { get; set; }

    public string? Status { get; set; }

    public decimal? LocationId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? OutDcLicensePlate { get; set; }

    public string? OutDcDriver { get; set; }

    public string? TrailerGroup { get; set; }

    public string? TrailerSize { get; set; }
}
