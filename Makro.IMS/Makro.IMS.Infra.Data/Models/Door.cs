using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Door
{
    public int InternalDoorId { get; set; }

    public string? WarehouseCode { get; set; }

    public string? DoorName { get; set; }

    public string? Sequence { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? LoadingType { get; set; }

    public string? DoorArea { get; set; }

    public decimal? BookingHeaderKey { get; set; }

    public string? TruckType { get; set; }
}
