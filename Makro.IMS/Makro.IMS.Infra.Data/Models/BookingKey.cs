using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class BookingKey
{
    public int InternalKeyId { get; set; }

    public string? CompanyCode { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModDate { get; set; }

    public string? UserStamp { get; set; }

    public bool? Active { get; set; }

    public DateTime? BookingDate { get; set; }

    public string? MailStatus { get; set; }

    public int? RecMailId { get; set; }
}
