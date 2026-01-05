using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.InterfaceDHLApi.Dtos
{
    public class SupGroupDto
    {
        public int InternalSupGroupId { get; set; }
        public string SupName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Country  { get; set; }
        public string ZipCode { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string UserDef1 { get; set; }
        public string UserDef2 { get; set; }
        public string UserDef3 { get; set; }
        public string UserDef4 { get; set; }
        public string UserDef5 { get; set; }
        public string UserDef6 { get; set; }
        public List<Supplier> Supplier { get; set; }
    }

    public  class Supplier
    {
        public string SupCode { get; set; }
        public string SupName { get; set; }
    }
}
