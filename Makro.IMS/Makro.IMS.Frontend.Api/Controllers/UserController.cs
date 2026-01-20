using Makro.IMS.Frontend.Api.Dto;
using Makro.IMS.Infra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;

namespace Makro.IMS.Frontend.Api.Controllers
{

    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        
        public UserController()
        {            
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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
                    

                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "user", this.Request.Headers.Authorization[0].Replace("Bearer",""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<UserMaster> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserMaster>>(resultData);

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


        //[HttpPost("Login")]
        //public async Task<ActionResult> Login(UserRegisterDto userRegister)
        //{
        //    try
        //    {
        //        var result = await _userMasterService.UserRegister(userRegister);

        //        return OkResponse(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex.Message);
        //    }
        //}


        [HttpPost("Resetpassword")]
        public async Task<ActionResult> ResetPassword(UserRegisterDto userRegister)
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

                    var resultApi = Service.HttpApiService.Post(apiUrl, "user/resetpassword", userRegister, this.Request.Headers.Authorization[0].Replace("Bearer", "").Replace("bearer",""));

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

        [HttpGet("translate")]
        public IEnumerable<Translate> GetTranslate()
        {
            var translates = new List<Translate>();

            translates.Add(new Translate { Code = "en", Name = "en" });
            translates.Add(new Translate { Code = "th", Name = "th" });

            return translates;                
        }

    }
}
