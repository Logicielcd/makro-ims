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
    
    public class GuardCheckInOutController : ApiControllerBase
    {
        private readonly GuardCheckInOutService _guardCheckInOutService;
        public GuardCheckInOutController(
            [FromServices] GuardCheckInOutService guardCheckInOutService
            )
        {
            _guardCheckInOutService = guardCheckInOutService;
        }

        [HttpGet("getPoCheckIn/{supCode}/{warehouseCode}")]
        public async Task<IActionResult> GetPoCheckIn(string supCode, string warehouseCode)
        {
            try
            {
                var result = await _guardCheckInOutService.GetBookingCheckIn(supCode);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        #region +++ Guard Check In +++

        [HttpGet("getGuardCheckIn/{bookingId}")]
        public async Task<IActionResult> GetBookingGuardCheckIn(string bookingId)
        {
            try
            {
                var result = await _guardCheckInOutService.GetBookingGuardCheckIn(bookingId);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("guardcheckinbooking")]
        public async Task<IActionResult> GuardCheckInBooking([FromBody] GuardCheckInOutDto checkIn)
        {
            try
            {
                var result = await _guardCheckInOutService.SaveGuardCheckIn(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        
        [HttpPost("guardCheckinByTruck")]
        public async Task<IActionResult> GuardCheckInByTruck([FromBody] GuardCheckInOutDto checkIn)
        {
            try
            {
                var result = await _guardCheckInOutService.SaveGuardCheckInByTruck(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        #endregion

        #region +++ Guard Check Out +++

        [HttpGet("getGuardCheckOut/{bookingId}")]
        public async Task<IActionResult> GetBookingGuardCheckOut(string bookingId)
        {
            try
            {
                var result = await _guardCheckInOutService.GetBookingGuardCheckOut(bookingId);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("guardCheckoutByTruck")]
        public async Task<IActionResult> GuardCheckOutByTruck([FromBody] GuardCheckInOutDto checkIn)
        {
            try
            {
                var result = await _guardCheckInOutService.SaveGuardCheckOutByTruck(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message); 
            }
        }

        [HttpPost("guardcheckoutbooking")]
        public async Task<IActionResult> GuardCheckOutBooking([FromBody] GuardCheckInOutDto data)
        {
            await _guardCheckInOutService.GuardCheckOut("",DateTime.Now,"");
            return OkResponse(true);
        }

        #endregion

        #region +++ Document Check In - Out +++

        [HttpPost("documentcheckinbooking")]
        public async Task<IActionResult> DocumentCheckInBooking([FromBody] CheckInDto checkIn)
        {
            try
            {
                var result = await _guardCheckInOutService.DocumentCheckInBooking(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        #endregion


        #region +++ Create booking Manual +++

        [HttpPost("manualbooking")]
        public async Task<IActionResult> ManualBooking([FromBody] ManualBookingDto booking)
        {
            try
            {
                var result = await _guardCheckInOutService.CreateManualBooking(booking);


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
