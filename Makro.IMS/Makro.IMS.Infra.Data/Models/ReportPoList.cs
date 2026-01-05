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
    public class ReportPoList
    {
        public string WarehouseCode { get; set; }
        public string CompanyCode { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public string? BookingId { get; set; }
        public DateTime? BookingStart { get; set; }
        public DateTime? BookingEnd { get; set; }
        public DateTime? BookingCreateDate { get; set; }
        public string? UserCreate { get; set; }
        public string PoNbr { get; set; }
        public DateTime PlanReceivedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime PoCreateDate { get; set; }
        public string? Status { get; set; }
    }

}
