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
    public class SupplierController : ApiControllerBase
    {
        
        public SupplierController()
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "supplier", this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<SupplierDto> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierDto>>(resultData);

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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "supplier/" + id, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Supplier d = Newtonsoft.Json.JsonConvert.DeserializeObject<Supplier>(resultData);

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

        [HttpGet("bysupgroup/{id}")]
        public async Task<IActionResult> GetBySupGroupId(int id)
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "supplier/bysupgroup/" + id.ToString(), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<SupplierDto> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SupplierDto>>(resultData);

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

        [HttpGet("supGroup/{id}")]
        public async Task<IActionResult> GetSupGroup(int id)
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "supplier/supGroup/" + id.ToString(), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        SupplierGroup d = Newtonsoft.Json.JsonConvert.DeserializeObject<SupplierGroup>(resultData);

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

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] SupplierDto supplier)
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

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "supplier/update", supplier, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        SupplierDto d = Newtonsoft.Json.JsonConvert.DeserializeObject<SupplierDto>(resultData);

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
