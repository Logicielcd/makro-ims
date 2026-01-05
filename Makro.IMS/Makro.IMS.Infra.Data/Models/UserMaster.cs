using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class UserMaster
{
    public string UserId { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Lastname { get; set; }

    public bool? Admin { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? Password { get; set; }

    public DateTime? ExpireDate { get; set; }

    public string? UserType { get; set; }

    public decimal? InternalSupGroupId { get; set; }

    public string? Approved { get; set; }

    public string? Email { get; set; }

    public string? SendEmail { get; set; }

    public string? WarehouseCode { get; set; }

    public string? OperationType { get; set; }

    public string? SupCode { get; set; }

    public string? Token { get; set; }
}
