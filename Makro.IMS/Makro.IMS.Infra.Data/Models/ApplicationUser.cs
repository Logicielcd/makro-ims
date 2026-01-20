
using System;

namespace Makro.IMS.Infra.Data.Models
{
    public class ApplicationUser
    {
        public string EmployeeNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public DateTime? DateOfEmployed { get; set; }
        /// <summary>
        /// Indicating the user is a system user.
        /// </summary>
        public bool IsSystemUser { get; set; }
        /// <summary>
        /// Indicating whether user is active or not.
        /// </summary>
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RefreshToken { get; set; }
    }
}
