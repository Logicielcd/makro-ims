using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Frontend.Api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System.Net;
using System.Reflection;

namespace Makro.IMS.Frontend.Api.Controllers
{

    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class DoorController : ApiControllerBase
    {
        
        public DoorController()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "door", this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<Door> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Door>>(resultData);

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

        [HttpGet("pages")]
        public async Task<IActionResult> GetPages([FromQuery] SieveModel sieveModel)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {
                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door/pages", sieveModel, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Sieve.PagedResult<Door> d = Newtonsoft.Json.JsonConvert.DeserializeObject<Sieve.PagedResult<Door>>(resultData);

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

        [HttpGet("warehouse/{warehouseCode}")]
        public async Task<IActionResult> GetByWarehouse(string warehouseCode)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "door/warehouse/" + warehouseCode, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<Door> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Door>>(resultData);

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

        [HttpGet("warehouse/{warehouseCode}/{supCode}")]
        public async Task<IActionResult> GetByWarehouseSup(string warehouseCode,string supCode)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "door/warehouse/" + warehouseCode + "/" + supCode, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<Door> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Door>>(resultData);

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "door/" + id.ToString(), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Door d = Newtonsoft.Json.JsonConvert.DeserializeObject<Door>(resultData);

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


        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Door door)
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

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door", door, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        bool d = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resultData);

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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Door door)
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

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door/update", door, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        bool d = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resultData);

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

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] int doorId)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;
                try
                {


                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door/delete", doorId, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        bool d = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resultData);

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

    }
}
