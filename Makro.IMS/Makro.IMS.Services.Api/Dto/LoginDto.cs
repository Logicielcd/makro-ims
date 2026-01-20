namespace Makro.IMS.Services.Api.Dto
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string SupCode { get; set; }
        public string Email { get; set; }

    }

    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDto UserInfo { get; set; }
    }

    public class UserRegisterPendingDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string SupCode { get; set; }
        public string Email { get; set; }

    }

}