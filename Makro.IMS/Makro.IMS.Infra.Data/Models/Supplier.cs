using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Supplier
{
    public int InternalSupId { get; set; }

    public int? InternalGroupId { get; set; }

    public string? SupCode { get; set; }

    public string? SupName { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? CompanyCode { get; set; }

    public int? InternalSupGroupId { get; set; }

    public string? RefDef1 { get; set; }

    public string? RefDef2 { get; set; }

    public string? RefDef3 { get; set; }

    public string? RefDef4 { get; set; }

    public DateTime? InterfaceDate { get; set; }
}
