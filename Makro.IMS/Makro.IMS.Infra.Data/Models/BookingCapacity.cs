using System.Reflection.Metadata.Ecma335;

namespace Makro.IMS.Infra.Data.Models
{

    public class BookingCapacity
    {
        //public string WarehouseCode { get; set; }
        //public DateTime BookingDateTime { get; set; }
        public string T { get; set; }
        public int? CON { get; set; }
        public int? NON { get; set; }
        public int? FULL_PL { get; set; }
        public decimal? CUBE_FULL { get; set; }
        public decimal? CUBE_CON { get; set; }
        public decimal? CUBE_NON { get; set; }
        
    }

}
