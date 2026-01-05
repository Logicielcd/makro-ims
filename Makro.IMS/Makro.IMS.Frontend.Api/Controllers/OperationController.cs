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
    public class OperationController : ApiControllerBase
    {
        
        public OperationController()
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "operation", this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<Operation> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Operation>>(resultData);

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
                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "operation/pages", sieveModel, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Sieve.PagedResult<Operation> d = Newtonsoft.Json.JsonConvert.DeserializeObject<Sieve.PagedResult<Operation>>(resultData);

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

        [HttpGet("{warehouseCode}")]
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "operation/" + warehouseCode, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<Operation> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Operation>>(resultData);

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

        [HttpGet("{operationName}/{warehouseCode}")]
        public async Task<IActionResult> GetByNameAndWhse(string operationName,string warehouseCode)
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


                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "operation/" + operationName + "/" + warehouseCode, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        Operation d = Newtonsoft.Json.JsonConvert.DeserializeObject<Operation>(resultData);

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

        

        //[HttpPost]
        //public async Task<IActionResult> Add([FromBody] Door door)
        //{

        //    try
        //    {
        //        var config = new ConfigurationBuilder()
        //        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        //        .AddJsonFile("appsettings.json").Build();

        //        var pdiApi = config.GetSection("ApiSetting");
        //        string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;
        //        try
        //        {

        //            var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door/add", door, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
        //            var resultData = "";

        //            if (resultPdiToken.StatusCode == HttpStatusCode.OK)
        //            {
        //                resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

        //                bool d = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resultData);

        //                return OkResponse(d);
        //            }
        //            else
        //            {
        //                resultData = resultPdiToken.Content.ReadAsStringAsync().Result;
        //                // error
        //                Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
        //                return BadResponse(r.Errors.Messages[0]);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadResponse(ex.Message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex.Message);
        //    }
        //}

        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] Door door)
        //{

        //    try
        //    {
        //        var config = new ConfigurationBuilder()
        //        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        //        .AddJsonFile("appsettings.json").Build();

        //        var pdiApi = config.GetSection("ApiSetting");
        //        string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;
        //        try
        //        {

        //            var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door/update", door, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
        //            var resultData = "";

        //            if (resultPdiToken.StatusCode == HttpStatusCode.OK)
        //            {
        //                resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

        //                bool d = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resultData);

        //                return OkResponse(d);
        //            }
        //            else
        //            {
        //                resultData = resultPdiToken.Content.ReadAsStringAsync().Result;
        //                // error
        //                Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
        //                return BadResponse(r.Errors.Messages[0]);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadResponse(ex.Message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex.Message);
        //    }
        //}

        //[HttpDelete("{doorId}")]
        //public async Task<IActionResult> Delete(int doorId)
        //{
        //    string userName = this.User.Identities.FirstOrDefault().Name;

        //    try
        //    {

        //        var config = new ConfigurationBuilder()
        //        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        //        .AddJsonFile("appsettings.json").Build();

        //        var pdiApi = config.GetSection("ApiSetting");
        //        string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;
        //        try
        //        {


        //            var resultPdiToken = Service.HttpApiService.Post(apiUrl, "door/delete", doorId, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
        //            var resultData = "";

        //            if (resultPdiToken.StatusCode == HttpStatusCode.OK)
        //            {
        //                resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

        //                bool d = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resultData);

        //                return OkResponse(d);
        //            }
        //            else
        //            {
        //                resultData = resultPdiToken.Content.ReadAsStringAsync().Result;
        //                // error
        //                Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
        //                return BadResponse(r.Errors.Messages[0]);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadResponse(ex.Message);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex.Message);
        //    }
        //}

    }
}
