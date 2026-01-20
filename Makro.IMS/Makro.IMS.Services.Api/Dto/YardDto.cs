namespace Makro.IMS.Services.Api.Dto
{
    public class YardDto
    {
    }

    public class YardMonitorDto
    {
        public decimal YardId { get; set; }
        public string? YardNo { get; set; }

        public string? YardZone { get; set; }

        public string? YardStatus { get; set; }

        public decimal? TrailerId { get; set; }

        public string? TrailerLicensePlate { get; set; }

        public string? TrailerStatus { get; set; }
    }
}
