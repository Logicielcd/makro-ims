using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Yard
{
    public decimal Id { get; set; }

    public string YardNo { get; set; } = null!;

    public string YardType { get; set; } = null!;

    public string YardZone { get; set; } = null!;

    public string? LocationNo { get; set; }

    public string? UserStamp { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? Status { get; set; }

    public string? TrailerType { get; set; }
}
