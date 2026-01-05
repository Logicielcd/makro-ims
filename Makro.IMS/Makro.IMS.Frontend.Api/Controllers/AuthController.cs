using Makro.IMS.Frontend.Api.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;

namespace Makro.IMS.Frontend.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {

        public AuthController()
        {

        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto login)
        {
            try
            {
                var result = "";

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

                try
                {

                    var config = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    .AddJsonFile("appsettings.json").Build();

                    var pdiApi = config.GetSection("ApiSetting");
                    string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                    try
                    {

                        var resultPdiToken = Service.HttpApiService.Post(apiUrl, "Auth/Login", login);
                        var resultData = "";

                        if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                        {
                            resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                            UserResponse d = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(resultData);

                            return OkResponse(d);
                        }
                        else
                        {
                            resultData = resultPdiToken.Content.ReadAsStringAsync().Result;
                            // error
                            Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
                            return BadResponse(r.Errors.Messages[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadResponse(ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    return BadResponse(ex.Message);
                }



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

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {

                    var resultApi = Service.HttpApiService.Post(apiUrl, "Auth/register", userRegister);

                    if (resultApi.StatusCode == HttpStatusCode.OK)
                    {
                        var resultData = resultApi.Content.ReadAsStringAsync().Result;

                        return OkResponse(true);
                    }
                    else
                    {
                        var resultData = resultApi.Content.ReadAsStringAsync().Result;
                        // error
                        Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
                        return BadResponse(r.Errors.Messages[0]);
                    }
                }
                catch (Exception ex)
                {
                    return BadResponse(ex.Message);
                }

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }


        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(TokenModel request)
        {

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "Auth/refresh-token", request);
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        UserResponse d = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(resultData);

                        return OkResponse(d);
                    }
                    else
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;
                        // error
                        Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
                        return BadResponse(r.Errors.Messages[0]);
                    }
                }
                catch (Exception ex)
                {
                    return BadResponse(ex.Message);
                }

            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }


        }

        [HttpPost("test")]
        public async Task<ActionResult> Test()
        {
            return OkResponse("1234");
        }

    }
}
