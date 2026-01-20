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
    
    public class PoController : ApiControllerBase
    {        
        private readonly PoService _poService;
        private readonly PoCommentService _poCommentService;
        public PoController(
            [FromServices] PoService poService,
            [FromServices] PoCommentService poCommentService
            )
        {
            _poService = poService;
            _poCommentService = poCommentService;
        }
        
        [HttpGet("{poNo}/{supcode}/{bookingDate}")]
        public async Task<IActionResult> GetByPo(string poNo, string supCode,DateTime bookingDate)
        {
            try
            {
                
                var result = await _poService.GetPoListByPo(poNo, supCode, "88",bookingDate.ToLocalTime());

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("{poNos}/{supcode}")]
        public async Task<IActionResult> GetByPos(string poNos, string supcode)
        {
            try
            {
                var result = await _poService.GetPoListByPos(poNos, supcode, "88");

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
                var result = await _poService.GetPoListBySupCodeAndCompany(supcode, "88",bookingDate.ToLocalTime());

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getpobypo/{poNos}/{supcode}")]
        public async Task<IActionResult> GetPoByPo(string poNos, string supcode)
        {
            try
            {
                var result = await _poService.GetPoByPo(poNos);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getcomment/{poNo}")]
        public async Task<IActionResult> GetComment(string poNo)
        {
            try
            {
                var result = await _poCommentService.GetByPo(poNo);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("addcomment")]
        public async Task<IActionResult> AddComment([FromBody] PoComment poComment)
        {
            try
            {
                var result = await _poCommentService.Update(poComment);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("dcdelay")]
        public async Task<IActionResult> DcDelay([FromBody] PoDcDelay poDcDelay)
        {
            try
            {
                var userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _poService.UpdateDcDelay(poDcDelay,userName);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getpolog/{poNo}")]
        public async Task<IActionResult> GetPoLog(string poNo)
        {
            try
            {
                var result = await _poService.GetPoLog(poNo);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("getPoMonitor")]
        public async Task<IActionResult> GetPoMonitor([FromBody] SieveModel sieveModel)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _poService.GetPoMonitor(sieveModel, userName);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("getPoMonitorExport")]
        public async Task<IActionResult> GetPoMonitorExport([FromBody] SieveModel sieveModel)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _poService.GetPoMonitorExport(sieveModel, userName);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
    }
}
