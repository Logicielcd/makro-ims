using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Services.Api.Dto
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Translate { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string? WarehouseCode { get; set; }
        public string? SupCode { get; set; }
        public string? OperationType { get; set; }
        public decimal? InternalSupGroupId { get; set; }
        
        public List<AuthMenu> AuthMenus { get; set; }
        public SupplierGroup? SupGroup { get; set; }
        public List<UserWarehouseDto>? UserWarehouses { get; set; }
        public List<UserSupplierGroupDto>? UserSupplierGroups { get; set; }
    }

    public class AuthMenu
    {
        public string Module { get; set; }
        public string PageName { get; set; }
        public string Action { get; set; }

    }

}
