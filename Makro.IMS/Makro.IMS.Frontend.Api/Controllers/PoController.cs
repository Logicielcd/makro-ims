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
    public class PoController : ApiControllerBase
    {        
      
        public PoController()
        {
            
        }

        [HttpGet("{poNo}/{supCode}/{bookingDate}")]
        public async Task<IActionResult> GetByPo(string poNo, string supCode,DateTime bookingDate)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            try
            {
                // call primary api

                var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json").Build();

                var pdiApi = config.GetSection("ApiSetting");
                string apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "apiUrl").Value;

                try
                {
                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "po/" + poNo + "/" + supCode + "/" + bookingDate.ToString("yyyy-MM-dd"), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        PoList d = Newtonsoft.Json.JsonConvert.DeserializeObject<PoList>(resultData);

                        return OkResponse(d);
                    }
                    else
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        //// error
                        // Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
                        //return BadResponse(r.Errors.Messages[0]);


                        // call secondary api

                        var secondaryApiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "secondaryApiUrl").Value;

                        var resultSecondaryApi = Service.HttpApiService.Get(secondaryApiUrl, "po/" + poNo + "/" + supCode + "/" + bookingDate.ToString("yyyy-MM-dd"), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                        var resultDataSecondaryApi = "";

                        if (resultSecondaryApi.StatusCode == HttpStatusCode.OK)
                        {
                            resultDataSecondaryApi = resultSecondaryApi.Content.ReadAsStringAsync().Result;                            

                            PoList d = Newtonsoft.Json.JsonConvert.DeserializeObject<PoList>(resultDataSecondaryApi);

                            // get warehouse information

                            var resultWhseApi = Service.HttpApiService.Get(apiUrl, "warehouse", this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                            var resultWhse = "";

                            if (resultWhseApi.StatusCode == HttpStatusCode.OK)
                            {
                                resultWhse = resultWhseApi.Content.ReadAsStringAsync().Result;
                                List<Warehouse> whses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Warehouse>>(resultWhse);
                                var whse = whses.FirstOrDefault(x => x.WarehouseMain == d.Warehouse_Code && x.CompanyCode == d.Company_Code);

                                d.Warehouse_Code = whse.WarehouseCode;
                                d.Company_Code = whse.CompanyCode;
                            }

                            return OkResponse(d);
                        }
                        else
                        {
                            resultDataSecondaryApi = resultSecondaryApi.Content.ReadAsStringAsync().Result;

                            // error
                            Dto.BadResponse r = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultDataSecondaryApi);
                            return BadResponse(r.Errors.Messages[0]);
                        }
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

        [HttpGet("getBySub/{supcode}/{bookingDate}")]
        public async Task<IActionResult> GetBySupCode(string supCode, DateTime bookingDate)
        {
            List<PoList> poLists = new List<PoList>();
            Dto.BadResponse badResponse = null;

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

                    var resultPdiToken = Service.HttpApiService.Get(apiUrl, "po/getBySub/" + supCode + "/" + bookingDate.ToString("yyyy-MM-dd"), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        List<PoList> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PoList>>(resultData);

                        poLists.AddRange(d);

                        //return OkResponse(d);
                    }
                    else
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;
                        // error
                        badResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultData);
                        //return BadResponse(r.Errors.Messages[0]);
                    }

                    apiUrl = pdiApi.GetChildren().FirstOrDefault(x => x.Key == "secondaryApiUrl").Value;

                    var resultSecondaryApi = Service.HttpApiService.Get(apiUrl, "po/getBySub/" + supCode + "/" + bookingDate.ToString("yyyy-MM-dd"), this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultDataSecondaryApi = "";

                    if (resultSecondaryApi.StatusCode == HttpStatusCode.OK)
                    {
                        resultDataSecondaryApi = resultSecondaryApi.Content.ReadAsStringAsync().Result;

                        List<PoList> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PoList>>(resultDataSecondaryApi);

                        poLists.AddRange(d);

                        //return OkResponse(d);
                    }
                    else
                    {
                        resultDataSecondaryApi = resultSecondaryApi.Content.ReadAsStringAsync().Result;
                        // error
                        badResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.BadResponse>(resultDataSecondaryApi);
                        //return BadResponse(r.Errors.Messages[0]);
                    }

                    if(poLists.Count > 0)
                    {
                        return OkResponse(poLists);
                    }
                    else
                    {
                        if(badResponse != null)
                        {
                            return BadResponse(badResponse.Errors.Messages[0]);
                        }
                        return OkResponse(null);
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
