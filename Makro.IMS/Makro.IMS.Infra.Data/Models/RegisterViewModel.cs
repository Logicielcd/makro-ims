using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Makro.IMS.Infra.Data.Models
{
    public class RegisterViewModel
    {
        public string EmployeeNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfEmployed { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<string> WcId { get; set; }

        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }

        public ApplicationUser ToApplicationUser(string createdBy)
        {
            return new ApplicationUser
            {
                EmployeeNo = EmployeeNo,
                FirstName = FirstName,
                LastName = LastName,
                Position = Position,
                //Email = Email,
                DateOfEmployed = DateOfEmployed,

                //UserName = Username,
                IsActive = IsActive.GetValueOrDefault(),
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        public IEnumerable<Claim> GetClaims() => WcId.Select(x => new Claim("WCID", x));

    }
}
