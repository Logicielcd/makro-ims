using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class DriverController : ApiControllerBase
    {
        private readonly DriverService _driverService;        
        

        public DriverController(
            [FromServices] DriverService driverService          
            )
        {
            _driverService = driverService;            
        }

        [HttpGet("getgatepass/{telno}")]
        public async Task<IActionResult> GetGatePassByPhoneNo(string telno) 
        {

            var result = await _driverService.GetGatePassByTelNo(telno);

            return OkResponse(result);
        }


    }
}
