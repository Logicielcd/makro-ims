using Makro.IMS.Infra.Data.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace Makro.IMS.Frontend.Api.Dto
{
    public class QueueManageSearchDto
    {
        public string WarehouseCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set;}
        public string? Status { get; set; }
        public string? OperationType { get; set; }
        public string? TruckType { get; set; }
    }
}
