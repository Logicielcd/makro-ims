using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.InterfaceDHLApi.Dtos
{
    public class TruckAssign
    {
        public string BookingId { get; set; }
        public string TruckType { get; set; }
        public string LicensePlate { get; set; }
        public string LicensePlate2 { get; set; }
        public string DriverName { get; set; }
        public string TelNo { get; set; }
        public string LineId { get; set; }
        public List<Pos> Pos { get; set; }       
    }

    public  class Pos
    {
        public string PoNbr { get; set; }        
    }
}
