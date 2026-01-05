using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Makro.IMS.Infra.Data.Models
{
    public class ApplicationRole
    {
        public static string Admin = "Admin";
        public static string SuperUser = "SuperUser";
        public static string User = "User";

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        //[NotMapped]
        //public List<AuthorizedMenu> AuthMenus { get; private set; } = new List<AuthorizedMenu>();
        //[NotMapped]
        //public List<string> Users { get; private set; } = new List<string>();

        //public void SetAuthorizedMenus(IEnumerable<AuthorizedMenu> authMenus)
        //{
        //    AuthMenus.Clear();

        //    AuthMenus.AddRange(authMenus);
        //}
    }
}
