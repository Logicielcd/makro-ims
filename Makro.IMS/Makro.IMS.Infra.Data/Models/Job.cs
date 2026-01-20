using System;
using System.Collections.Generic;

namespace Makro.IMS.Infra.Data.Models;

public partial class Job
{
    public int Id { get; set; }

    public string? JobId { get; set; }

    public string? JobType { get; set; }

    public DateTime? JobDate { get; set; }

    public decimal? TrailerId { get; set; }

    public decimal? ShuntId { get; set; }

    public decimal? LocationId { get; set; }

    public string? LocationType { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UserStamp { get; set; }

    public DateTime? ModDate { get; set; }

    public DateTime? ProcessTime { get; set; }
}
