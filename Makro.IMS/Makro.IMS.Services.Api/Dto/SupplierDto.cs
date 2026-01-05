namespace Makro.IMS.Services.Api.Dto
{

    public class SupplierDto
    {
        public int InternalSupId { get; set; }
        public int? InternalGroupId { get; set; }
        public string? SupCode { get; set; }
        public string? SupName { get; set; }
        public string? CompanyCode { get; set; }
        public int? InternalSupGroupId { get; set; }
        public string? Remark { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEMail { get; set; }
        public string? MobileNumber { get; set; }
    }

}
