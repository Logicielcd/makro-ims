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
    public class WarehouseController : ApiControllerBase
    {
        
        public WarehouseController()
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
                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "warehouse", this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<Warehouse> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Warehouse>>(resultData);

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
                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "warehouse/pages", sieveModel, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Sieve.PagedResult<Warehouse> d = Newtonsoft.Json.JsonConvert.DeserializeObject<Sieve.PagedResult<Warehouse>>(resultData);

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
        public async Task<IActionResult> GetById(string id)
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "warehouse/" + id, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Warehouse d = Newtonsoft.Json.JsonConvert.DeserializeObject<Warehouse>(resultData);

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
        public async Task<IActionResult> Add([FromBody] Warehouse warehouse)
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

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "warehouse/add", warehouse, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
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
        public async Task<IActionResult> Update([FromBody] Warehouse warehouse)
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

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "warehouse/update", warehouse, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
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

        [HttpDelete("{warehouseCode}")]
        public async Task<IActionResult> Delete(string warehouseCode)
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


                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "warehouse/delete", warehouseCode, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
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
