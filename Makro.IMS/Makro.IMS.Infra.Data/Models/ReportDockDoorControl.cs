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
    public class ReportDockDoorControl
    {
        public string DoorName { get; set; }
        public string DoorArea { get; set; }
        public string DoorType { get; set; }
        public string? SupCode { get; set; }
        public string? SupName { get; set; }
        public string? LicensePlate { get; set; }
        public string? TruckType { get; set; }
        public DateTime? AssignQueueTime { get; set; }
        public DateTime? OnDockTime { get; set; }
        public int? ProcessTime { get; set; }
        public string? Sequence { get; set; }
    }

}
