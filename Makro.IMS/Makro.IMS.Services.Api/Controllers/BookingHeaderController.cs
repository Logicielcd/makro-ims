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
    
    public class BookingHeaderController : ApiControllerBase
    {
        private readonly BookingHeaderService _bookingHeaderService;
        public BookingHeaderController(
            [FromServices] BookingHeaderService bookingHeaderService
            )
        {
            _bookingHeaderService = bookingHeaderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var result = await _bookingHeaderService.GetBookingsAsync();
            //result.ForEach(x => x. = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages([FromBody] SieveModel sieveModel)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _bookingHeaderService.GetBookingsPaged(sieveModel, userName);
            //result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpPost("precheckinpages")]
        public async Task<IActionResult> GetPreCheckInPages([FromBody] SieveModel sieveModel)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _bookingHeaderService.GetPreCheckInPaged(sieveModel, userName);
            //result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpPost("precheckincompletedpages")]
        public async Task<IActionResult> GetPreCheckInCompletedPages([FromBody] SieveModel sieveModel)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _bookingHeaderService.GetPreCheckInCompletedPaged(sieveModel, userName);
            //result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpPost("precheckinpagesexport")]
        public async Task<IActionResult> GetPreCheckInPagesExport([FromBody] SieveModel sieveModel)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _bookingHeaderService.GetPreCheckInPagedExport(sieveModel, userName);
            //result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }


        [HttpGet("dashboard/{warehouseCode}")]
        public async Task<IActionResult> GetDashboard(string warehouseCode)
        {
            var result = await _bookingHeaderService.GetDashboard(warehouseCode);
            //result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookingHeaderService.GetBookingsByIdAsync(id);
            //result.BookingDate = result.BookingDate.Date;
            return OkResponse(result);
        }


        [HttpGet("getByPo/{poNo}/{supCode}")]
        public async Task<IActionResult> GetByPo(string poNo, string supCode)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingByPo(poNo, supCode);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getByBooking/{bookingId}/{supCode}")]
        public async Task<IActionResult> GetByBooking(string bookingId, string supCode)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingByBooking(bookingId, supCode);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getBookingByBooking/{bookingId}")]
        public async Task<IActionResult> GetBookingByBooking(string bookingId)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingByBookingId(bookingId);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getByBookingHeaderKey/{bookingHeaderKey}/{supCode}")]
        public async Task<IActionResult> GetByBookingHeaderKey(int bookingHeaderKey, string supCode)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingByBookingHeaderKey(bookingHeaderKey, supCode);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("getBySupAndBookingDate/{supCode}/{bookingDate}")]
        public async Task<IActionResult> GetBySupAndBookingDate(string supCode,DateTime bookingDate)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingHeaderBySupCodeAndBookingDate(supCode,bookingDate.Date);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
        

        [HttpGet("getPoCheckIn/{supCode}/{warehouseCode}")]
        public async Task<IActionResult> GetPoCheckIn(string supCode, string warehouseCode)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingCheckIn(supCode, supCode);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] BookingTruckCheckInDto bookingCheckIn)
        {
            try
            {
                var result = await _bookingHeaderService.SaveCheckIn(bookingCheckIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("checkinpo")]
        public async Task<IActionResult> CheckInPo([FromBody] CheckInDto checkIn)
        {
            try
            {
                var result = await _bookingHeaderService.CheckInPo(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("checkinbooking")]
        public async Task<IActionResult> CheckInBooking([FromBody] CheckInDto checkIn)
        {
            try
            {
                var result = await _bookingHeaderService.CheckInBooking(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet("getPoCheckOut/{poNo}/{supCode}")]
        public async Task<IActionResult> GetPoCheckOut(string poNo, string supCode)
        {
            try
            {
                var result = await _bookingHeaderService.GetPoCheckOut(poNo, supCode);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] BookingCheckOutDto bookingCheckOut)
        {
            try
            {
                var result = await _bookingHeaderService.SaveCheckOut(bookingCheckOut);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("precheckin")]
        public async Task<IActionResult> PreCheckIn([FromBody] PreCheckIn bookingCheckIn)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                

                var result = await _bookingHeaderService.SavePreCheckIn(bookingCheckIn);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("deletebookingheader")]
        public async Task<IActionResult> DeleteBookingHeader([FromBody] int bookingHeaderId)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.DeleteBookingHeader(bookingHeaderId,userName);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deletebookingheaderall")]
        public async Task<IActionResult> DeleteBookingHeaderAll([FromBody] int bookingHeaderId)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.DeleteBookingHeaderAll(bookingHeaderId,userName);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deleteprecheckin")]
        public async Task<IActionResult> DeletePreCheckIn([FromBody] int bookingHeaderId)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.DeletePreCheckIn(bookingHeaderId,userName);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("approvebooking")]
        public async Task<IActionResult> ApproveBooking([FromBody] int bookingHeaderId)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.ApprovedBooking(bookingHeaderId,userName);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("backhaulbooking")]
        public async Task<IActionResult> BackhaulBooking([FromBody] BackhaulBookingDto backhaulBookingDto)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.BackHaulBooking(backhaulBookingDto.BookingHeaderId, backhaulBookingDto.IsBackhaul,userName);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("dcdelaybooking")]
        public async Task<IActionResult> DcDelayBooking([FromBody] DcDelayBookingDto dcDelayBookingDto)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.DcDelayBooking(dcDelayBookingDto.BookingHeaderId, dcDelayBookingDto.BookingDetails, userName);

                return OkResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getByBookingId/{bookingId}")]
        public async Task<IActionResult> GetByBookingId(string bookingId)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingByBookingId(bookingId);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("bookinglog/{internaltruckid}")]
        public async Task<IActionResult> BookingLog(int internaltruckid)
        {
            try
            {
                var result = await _bookingHeaderService.GetBookingLog(internaltruckid);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("cancelbooking")]
        public async Task<IActionResult> CancelBooking([FromBody] BookingHeaderDto bookingHeaderDto)
        {
            try
            {

                string userName = this.User.Identities.FirstOrDefault().Name;
                var result = await _bookingHeaderService.CancelBooking(bookingHeaderDto,userName);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
    }
}
