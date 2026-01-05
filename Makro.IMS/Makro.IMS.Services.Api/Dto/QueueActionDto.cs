using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Services.Api.Dto
{
    public class QueueActionDto
    {
        public int InternalHeaderKey { get; set; }
        public string QueueNo { get; set; }
        public string? Status { get; set; }
        public int? InternalDoorId { get; set; }
        public int? NewInternalDoorId { get; set; }
        public int? InternalTruckCheckInId { get; set; }
        public string? Remark { get; set; }
        public string? UserName { get; set; }

    }
}
