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
    
    public class EstTimeController : ApiControllerBase
    {
        private readonly EstTimeService _estTimeService;
        public EstTimeController(
            [FromServices] EstTimeService estTimeService
            )
        {
            _estTimeService = estTimeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _estTimeService.GetEstTimes();

            return OkResponse(result);
        }

        [HttpGet("supGroupId/{supGroupId}")]
        public async Task<IActionResult> GetByWarehouse(int supGroupId)
        {
            var result = await _estTimeService.GetEstTimeBySupGroupId(supGroupId);

            return OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EstTime est)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;
            
            var result = await _estTimeService.AddEst(est);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] dynamic estTimeId)
        {
            dynamic objectDoor = Newtonsoft.Json.JsonConvert.DeserializeObject(estTimeId.ToString());
            int id = objectDoor.estTimeId;
            var result = await _estTimeService.Delete(id);
            return OkResponse(result);
        }
    }
}
