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
    
    public class SlotCapacityController : ApiControllerBase
    {
        private readonly SlotCapacityService _slotCapacityService;
        public SlotCapacityController(
            [FromServices] SlotCapacityService slotCapacityService
            )
        {
            _slotCapacityService = slotCapacityService;
        }
        
        [HttpGet("{bookingdate}/{warehouse}/{operationtype}")]
        public async Task<IActionResult> GetSlotCapacity(DateTime bookingdate, string warehouse,string operationtype)
        {
            var result = await _slotCapacityService.GetSlotCapacity(bookingdate, warehouse,operationtype);
            //result.ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }


    }
}
