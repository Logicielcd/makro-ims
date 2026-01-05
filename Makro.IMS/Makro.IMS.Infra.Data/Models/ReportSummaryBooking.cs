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
    public class ReportSummaryBooking
    {
        public decimal TotalBooking { get; set; }
        public decimal TotalCheckIn { get; set; }
        public decimal TotalTodayCheckIn { get; set; }
        public decimal TotalQueue { get; set; }
        public decimal TotalUnloading { get; set; }
        public decimal TotalUnloaded { get; set; }
        public decimal TotalWaitingDoc { get; set; }
        public decimal TotalWaitingCheckOut { get; set; }
        public decimal TotalCheckOut { get; set; }
        public decimal TotalInWhse { get; set; }
    }

}
