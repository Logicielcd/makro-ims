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
    
    public class TruckMasterController : ApiControllerBase
    {
        private readonly TruckMasterService _truckMasterService;
        public TruckMasterController(
            [FromServices] TruckMasterService truckMasterService
            )
        {
            _truckMasterService = truckMasterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _truckMasterService.GetTrucks();

            return OkResponse(result);
        }

        [HttpGet("getTruckCapAll")]
        public async Task<IActionResult> GetTruckCapAll()
        {
            var result = await _truckMasterService.GetTruckCapAll();

            return OkResponse(result);
        }

        [HttpGet("getTruckRuleAll")]
        public async Task<IActionResult> GetTruckRuleAll()
        {
            var result = await _truckMasterService.GetTruckRules();

            return OkResponse(result);
        }
    }
}
