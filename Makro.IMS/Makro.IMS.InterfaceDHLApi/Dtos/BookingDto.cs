using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.InterfaceDHLApi.Dtos
{
    public class BookingDto
    {
        public string WarehouseCode { get; set; }
        public string BookingId { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public string Status { get; set; }
        public string DoorName { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime BookingStart { get; set; }
        public DateTime BookingEnd { get; set; }
        public DateTime ModDate { get; set; }
        public string UserStamp { get; set; }
        public string BackHaul { get; set; }
        public DateTime FirstBookginStart { get; set; }
        public DateTime FirstBookingEnd { get; set; }
        public string FirstUserStamp { get; set; }
        public string Postponed { get; set; }
        public string TruckType { get; set; }
        public string UserDef1 { get; set; }
        public string UserDef2 { get; set; }
        public string UserDef3 { get; set; }
        public string UserDef4 { get; set; }
        public int TotalQTY { get; set; }
        public string MerchType { get; set; }
        public string Remark { get; set; }
        public string RemarkPostponed { get; set; }
        public string RemarkOvercap { get; set; }
        public List<booking_betails> booking_betails { get; set; }
        public List<Booking_trucks> booking_Trucks { get; set; }
    }

    public  class booking_betails
    {
        public string PoNbr { get; set; }
        public int TotalQty { get; set; } = 0;
        public DateTime PlanRec { get; set; }
        public decimal Full { get; set; } = 0;
        public decimal Con { get; set; } = 0;
        public decimal Non { get; set; } = 0;
        public decimal Cube { get; set; } = 0;
        public decimal CubeFull { get; set; } = 0;
        public decimal CubeCon { get; set; } = 0;
        public decimal CubeNon { get; set; } = 0;
        public string MerchType { get; set; }
        public string Postponed { get; set; }
        public string DelayReason { get; set; }
    }

    public class Booking_trucks
    {
        public string Truck_Type { get; set; }
        public int Total_Truck { get; set; }
    }

}
