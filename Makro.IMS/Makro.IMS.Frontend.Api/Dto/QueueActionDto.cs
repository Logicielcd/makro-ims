namespace Makro.IMS.Frontend.Api.Dto
{
    public class QueueActionDto
    {
        public int InternalHeaderKey { get; set; }
        public string QueueNo { get; set; }
        public string? Status { get; set; }
        public int? InternalDoorId { get; set; }
        public int? InternalTruckCheckInId { get; set; }
    }
}
