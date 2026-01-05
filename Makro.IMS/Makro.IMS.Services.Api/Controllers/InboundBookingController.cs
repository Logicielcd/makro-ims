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
    
    public class InboundBookingController : ApiControllerBase
    {
        private readonly InboundBookingService _inboundBookingService;
        public InboundBookingController(
            [FromServices] InboundBookingService inboundBookingService
            )
        {
            _inboundBookingService = inboundBookingService;
        }

        #region +++ Create inbound booking +++

        [HttpPost("inboundbooking")]
        public async Task<IActionResult> InboundBooking([FromBody] InboundBookingDto booking)
        {
            try
            {
                var result = await _inboundBookingService.CreateInboundBooking(booking);


                var strResult = new { result };

                return OkResponse(strResult);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        #endregion
    }
}
