using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class BookingKeyController : ApiControllerBase
    {
        private readonly BookingKeyService _bookingKeyService;
        public BookingKeyController(
            [FromServices] BookingKeyService bookingKeyService
            )
        {
            _bookingKeyService = bookingKeyService;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookingKeyDto bookingKey)
        {
            try
            {
                var userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingKeyService.SaveBooking(bookingKey, userName);

                var strResult = new {result};

                return OkResponse(strResult);
            }
            catch(Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("updateBooking")]
        public async Task<IActionResult> UpdateBooking([FromBody] BookingKeyDto bookingKey)
        {
            try
            {
                var userName = this.User.Identities.FirstOrDefault().Name;
                bookingKey.UserName = userName;
                var result = await _bookingKeyService.UpdateBooking(bookingKey,userName);

               // var strResult = new { result };

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _bookingKeyService.GetBookingKeyByBookingKeyId(id);
                //result.BookingDate = result.BookingDate.Date;
                return OkResponse(result);
            }
            catch(Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


    }
}
