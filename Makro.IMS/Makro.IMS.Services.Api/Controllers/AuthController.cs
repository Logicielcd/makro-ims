using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ApiControllerBase
    {
        private readonly UserMasterService _userMasterService;

        public AuthController(
            [FromServices] UserMasterService userMasterService
            )
        {
            _userMasterService = userMasterService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto login)
        {
            try
            {
                var result = await _userMasterService.Login(login);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.UserRegister(userRegister);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(TokenModel request)
        {
            //LoginDto login = new LoginDto();
            //login.UserName = request.UserInfo.UserName;
            //login.Password = request.UserInfo.UserName;
            var result = await _userMasterService.RefreshToken(request);

            return OkResponse(result);
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult> ValidateToken(TokenModel request)
        {
            //LoginDto login = new LoginDto();
            //login.UserName = request.UserInfo.UserName;
            //login.Password = request.UserInfo.UserName;
            var result = await _userMasterService.ValidateToken(request);

            return OkResponse(result);
        }

        [HttpPost("test")]
        public async Task<ActionResult> Test(LoginDto login)
        {
            return OkResponse("1234");
        }

        [HttpPost("LoginDriver")]
        public async Task<ActionResult> LoginDriver(LoginDto login)
        {
            try
            {
                var result = await _userMasterService.LoginDriver(login);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
    }
}
