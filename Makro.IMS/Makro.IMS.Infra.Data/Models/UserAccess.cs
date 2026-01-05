using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class UserAccess
{
    public int MenuId { get; set; }

    public string UserId { get; set; } = null!;

    public string? FromName { get; set; }
}
