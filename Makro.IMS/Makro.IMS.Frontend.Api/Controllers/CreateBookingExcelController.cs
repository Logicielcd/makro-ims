using Makro.IMS.Frontend.Api.Dto;
using Makro.IMS.Infra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;

namespace Makro.IMS.Frontend.Api.Controllers
{

    [Route("api/[controller]")]    
    [ApiController]
    public class CreateBookingExcelController : ApiControllerBase
    {        
      
        public CreateBookingExcelController()
        {
            
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] CreateBookingExcelDto createBookingExcelDto)
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

                    var resultPdiToken = Service.HttpApiService.Post(apiUrl, "createbookingexcel/import", createBookingExcelDto, this.Request.Headers.Authorization[0].Replace("bearer", "").Replace("Bearer", ""));
                    var resultData = "";

                    if (resultPdiToken.StatusCode == HttpStatusCode.OK)
                    {
                        resultData = resultPdiToken.Content.ReadAsStringAsync().Result;

                        CreateBookingExcelDto d = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateBookingExcelDto>(resultData);

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
