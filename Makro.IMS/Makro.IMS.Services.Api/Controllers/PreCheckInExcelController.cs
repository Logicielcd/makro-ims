using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class PreCheckInExcelController : ApiControllerBase
    {        
        private readonly PreCheckInExcelService _preCheckInExcelService;
        public PreCheckInExcelController(
            [FromServices] PreCheckInExcelService preCheckInExcelService
            )
        {
            _preCheckInExcelService = preCheckInExcelService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] PreCheckInExcelDto preCheckInExcel)
        {
            try
            {
                var userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _preCheckInExcelService.Import(preCheckInExcel,userName);
                
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

    }
}
