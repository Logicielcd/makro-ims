using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class SupTypeMaster
{
    public int InternalSupTypeId { get; set; }

    public string? CompanyCode { get; set; }

    public string? SupType { get; set; }

    public int? BackColor { get; set; }

    public int? FontColor { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public bool? Backhaul { get; set; }
}
