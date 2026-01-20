using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class PoComment
{
    public string Po { get; set; } = null!;

    public string? Comment1 { get; set; }

    public string? Comment2 { get; set; }

    public string? Comment3 { get; set; }

    public string? Comment4 { get; set; }

    public string? Comment5 { get; set; }

    public string? Comment6 { get; set; }

    public string? Comment7 { get; set; }

    public string? Comment8 { get; set; }

    public DateTime? DateTimeStamp { get; set; }

    public DateTime? InterfaceDate { get; set; }
}
