using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Models
{
    public class ReportTruckStatus
    {
        public int RowNo { get; set; }
        public DateTime BookingDate { get; set; }
        public string? BookingId { get; set; }
        public string? LicensePlate { get; set; }
        public string? SupCode { get; set; }
        public string? SupName { get; set; }
        public string? MerchType { get; set; }
        public string? TotalWeight { get; set; }
        public string? SlotBooking { get; set; }
        public string? Door { get; set; }
        public string? QueueSeq { get; set; }
        public string? Status { get; set; }
    }

}
