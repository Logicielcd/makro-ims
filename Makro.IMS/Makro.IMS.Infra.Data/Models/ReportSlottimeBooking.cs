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
    public class ReportSlottimeBooking
    {
        public DateTime BookingDate { get; set; }
        public string? MerchType { get; set; }
        public string? DataType { get; set; }
        public DateTime SlotTime { get; set; }
        public decimal? Quantity { get; set; }
        
    }

}
