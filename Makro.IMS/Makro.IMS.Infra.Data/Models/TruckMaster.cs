using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class TruckMaster
{
    public int InternalTruckId { get; set; }

    public string? Company { get; set; }

    public string? TruckCode { get; set; }

    public string? TruckName { get; set; }

    public string? Sequence { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? TruckGroup { get; set; }
}
