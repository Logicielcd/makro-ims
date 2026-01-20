using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class UserWarehouse
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? WarehouseCode { get; set; }
}
