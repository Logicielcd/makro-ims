using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    
    public class BookingController : ApiControllerBase
    {
        private readonly BookingKeyService _bookingKeyService;
        private readonly PoService _poService;
        public BookingController(
            [FromServices] BookingKeyService bookingKeyService,
            [FromServices] PoService poService
            )
        {
            _bookingKeyService = bookingKeyService;
            _poService = poService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookingKeyService.GetBookingsAsync();

            return OkResponse(result);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int keyId)
        {
            var result = await _bookingKeyService.GetBookingsByIdAsync(keyId);

            return OkResponse(result);
        }

        [HttpGet("GetBySupCodeAndBookingDate")]
        public async Task<IActionResult> GetBySupCodeAndBookingDate(string supCode,DateTime bookingDate)
        {
            var result = await _bookingKeyService.GetBookingHeaderBySupCodeAndBookingDate(supCode,bookingDate);

            return OkResponse(result);
        }
        
        [HttpGet("GetBySupCodeAndCompany")]        
        public async Task<IActionResult> GetPoListBySupCodeAndCompany(string supCode, string companyCode)
        {
            var result = await _poService.GetPoListBySupCodeAndCompany(supCode, companyCode,DateTime.Now);
            return OkResponse(result);
        }


    }
}
