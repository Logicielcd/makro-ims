namespace Makro.IMS.Services.Api.Dto
{
    public class UserResponse
    {
        
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserDto UserInfo { get; set; }
        public string RefreshToken { get; set; }

    }
}
