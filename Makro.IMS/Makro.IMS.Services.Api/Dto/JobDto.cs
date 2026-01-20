using System.Diagnostics;
using System.Reflection.Metadata;

namespace Makro.IMS.Services.Api.Dto
{
    public class JobDto
    {
        public int Id { get; set; }
        public string? JobId { get; set; }
        public string? JobType { get; set; }
        public DateTime? JobDate { get; set; }

        // trailer
        public decimal? TrailerId { get; set; }
        public string? TrailerLicensePlate { get; set; }
        public string? TrailerType { get; set; }
        public string? TrailerDriver { get; set; }
        public string? TrailerTel { get; set; }

        public decimal? FromYardId { get; set; }
        public string? FromYardNo { get; set; }
        public string? FromYardType { get; set; }

        // shunt
        public decimal? ShuntId { get; set; }
        public string? ShuntLicensePlate { get; set; }
        public string? ShuntDriver { get; set; }
        public string? ShuntTel { get; set; }

        // yard destination
        public decimal? YardId { get; set; }
        public string? YardNo { get; set; }
        public string? YardLocationNo { get; set; }
        public string? YardType { get; set; }
        public string? YardZone { get; set; }

        // door
        public string? LocationType { get; set; }

        // status
        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UserStamp { get; set; }
        public DateTime? ModDate { get; set; }
    }

    public class JobOnDock
    {
        // location
        public decimal? LocationId { get; set; }
        public string? LocationNo { get; set; }
        public string? LocationZone { get; set; }
        public string LocationType { get; set; }
        public string? LocationStatus { get; set; }

        // Job 
        public int Id { get; set; }
        public string? JobId { get; set; }
        public string? Status { get; set; }

        // trailer
        public decimal? TrailerId { get; set; }
        public string? TrailerLicensePlate { get; set; }
        public string? TrailerStatus { get; set; }
        public string? TotalProcessTime { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? ModDate { get; set; }
    }

    public class ChangeDoorRequestDto
    {
        public int JobId { get; set; }
        public decimal LocationId { get; set; }
    }

    public class ChangeLocationRequest
    {
        public decimal TrailerId { get; set; }
        public decimal LocationId { get; set; }
    }

    public class ChangeTrailerOnDockDto
    {
        public string JobId { get; set; }
        public decimal TrailerId { get; set; }
        public decimal ShuntId { get; set; }
        public decimal LocationId { get; set; }
    }

    public class OutDCRequestDto
    {
        public decimal TrailerId { get; set; }
        public string OutDcLicensePlate { get; set; }
        public string OutDcDriver { get; set; }
    }
}