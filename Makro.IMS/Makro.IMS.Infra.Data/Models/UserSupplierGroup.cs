using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class UserSupplierGroup
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int InternalSupGroup { get; set; }
}
