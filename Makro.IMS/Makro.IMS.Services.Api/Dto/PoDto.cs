using System.DirectoryServices.Protocols;

namespace Makro.IMS.Services.Api.Dto
{

    public class PoDto
    {
        public int InternalPoNo { get; set; }
        public string CompanyCode { get; set; }
        public string WarehouseCode { get; set; }
        public string PoNo { get; set; }
        public string SupCode { get; set; }
        public string MerchType { get; set; }
        public int TotalQty { get; set; }
        public DateTime PlanReceiveDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Remark { get; set; }
        public int Booker { get; set; }
        public int Full { get; set; }
        public int Con { get; set; }
        public int Non { get; set; }
        public decimal CubeFull { get; set; }
        public decimal CubeCon { get; set; }
        public decimal CubeNon { get; set; }
        public decimal Weight { get; set; }
        public string UserDef1 { get; set; }
        public string UserDef2 { get; set; }

        public string UserDef3 { get; set; }
        public string UserDef4 { get; set; }
        public string CompanyName { get; set; }
        public string WarehouseName { get; set; }
        public int FullPl { get; set; }

    }

    public class PoDtos
    {
        public List<PoDto> Pos { get; set; }
    }

    public class PoDcDelay
    {
        public string PoNo { get; set; }
        public string DelayReason { get; set; }
    }
}
