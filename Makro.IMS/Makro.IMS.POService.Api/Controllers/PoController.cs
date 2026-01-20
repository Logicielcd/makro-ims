using Makro.IMS.POServices.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.POServices.Api.Controllers
{

    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class PoController : ApiControllerBase
    {        
        private readonly PoService _poService;
        public PoController(
            [FromServices] PoService poService
            )
        {
            _poService = poService;
        }


        [HttpGet("{poNo}/{supCode}/{bookingDate}")]
        public async Task<IActionResult> GetByPo(string poNo, string supCode,DateTime bookingDate)
        {
            try
            {
                var result = await _poService.GetPoListByPoCDC(poNo, supCode, "88",bookingDate.ToLocalTime());

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet("getBySub/{supcode}/{bookingDate}")]
        public async Task<IActionResult> GetBySupCode(string supcode,DateTime bookingDate)
        {
            try
            {
                var result = await _poService.GetPoListBySupCodeAndCompanyCDC(supcode, "88",bookingDate.ToLocalTime());

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


    }
}
