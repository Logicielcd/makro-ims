namespace Makro.IMS.Services.Api.Dto
{
    public class UserMasterDto
    {
        public string UserId { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Lastname { get; set; }
        public bool? Admin { get; set; }
        public string? Password { get; set; }
        public string? UserType { get; set; }
        public decimal? InternalSupGroupId { get; set; }
        public string? Approved { get; set; }
        public string? Email { get; set; }
        public string? WarehouseCode { get; set; }
        public string? OperationType { get; set; }
        public string? SupCode { get; set; }

        public List<UserWarehouseDto>? userWarehouses { get; set; } = new List<UserWarehouseDto>();
        public List<UserSupplierGroupDto>? userSupplierGroups { get; set; } = new List<UserSupplierGroupDto>();
    }

    public class UserWarehouseDto
    {        
        public string? WarehouseCode { get; set; }
    }

    public class UserSupplierGroupDto 
    {
        public decimal InternalSupGroupId { get; set; }
    }

}
