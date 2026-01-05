using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class PoLog
{
    public decimal Id { get; set; }

    public string PoNbr { get; set; } = null!;

    public string? UserStamp { get; set; }

    public DateTime? DateTimeStamp { get; set; }

    public string? Log { get; set; }
}
