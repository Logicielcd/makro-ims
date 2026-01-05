using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Company
{
    public string CompanyCode { get; set; } = null!;

    public string? CompanyName { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public string? ReportPath { get; set; }

    public string? PdfPath { get; set; }

    public string? RecAttPath { get; set; }

    public string? ReadEMailFolder { get; set; }

    public string? EMailGateway { get; set; }

    public string? EMailSender { get; set; }
}
