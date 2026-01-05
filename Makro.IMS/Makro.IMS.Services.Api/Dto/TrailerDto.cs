namespace Makro.IMS.Services.Api.Dto
{
    public class TrailerDto
    {
        public decimal Id { get; set; }
        public string? LicensePlate { get; set; }
        public string? TrailerType { get; set; }
        public string? TrailerGroup { get; set; }
        public string? TrailerSize { get; set; }
        public string? Status { get; set; }
        public decimal? LocationId { get; set; }
        public string? LocationNo { get; set; }
        public string? LocationType { get; set; }
        public string? JobStatusOriginal { get; set; }
        public string? JobStatus { get; set; }
        public string? JobDockStatus { get; set; }
        public string? LocationZone { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? UserStamp { get; set; }
    }
}
