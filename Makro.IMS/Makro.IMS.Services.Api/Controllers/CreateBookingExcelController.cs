using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class CreateBookingExcelController : ApiControllerBase
    {        
        private readonly CreateBookingExcelService _createBookingExcelService;
        public CreateBookingExcelController(
            [FromServices] CreateBookingExcelService createBookingExcelService
            )
        {
            _createBookingExcelService = createBookingExcelService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] CreateBookingExcelDto createBookingExcel)
        {
            try
            {
                var userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _createBookingExcelService.Import(createBookingExcel,userName);
                
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

    }
}
