namespace Makro.IMS.Services.Api.Dto
{
    public class TruckCheckInDto
    {
        public string LicensePlate { get; set; }
        public string BookingId { get; set; }
        public DateTime CheckInDateTime { get; set; }
    }

    public class TruckCheckOutDto
    {
        public string LicensePlate { get; set; }        
        public DateTime CheckInDateTime { get; set; }
    }


}