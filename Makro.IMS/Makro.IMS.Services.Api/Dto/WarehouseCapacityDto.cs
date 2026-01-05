namespace Makro.IMS.Services.Api.Dto
{

    public class WarehouseCapacityDto
    {
        public string WarehouseCode { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int? CON { get; set; }
        public int? NON { get; set; }
        public int? FULL_PL { get; set; }
        public decimal? CUBE_FULL { get; set; }
        public decimal? CUBE_CON { get; set; }
        public decimal? CUBE_NON { get; set; }
        public int? MAX_CON { get; set; }
        public int? MAX_NON { get; set; }
        public int? MAX_FULL_PL { get; set; }
        public decimal? MAX_CUBE_FULL { get; set; }
        public decimal? MAX_CUBE_CON { get; set; }
        public decimal? MAX_CUBE_NON { get; set; }

    }

}
