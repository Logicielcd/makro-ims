using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class InterfaceController : ApiControllerBase
    {        
        private readonly InterfaceService _interfaceService;
        public InterfaceController(
            [FromServices] InterfaceService interfaceService
            )
        {
            _interfaceService = interfaceService;
        }

        [HttpPost("PO")]
        public async Task<IActionResult> InterfacePO([FromBody] PoDtos pos)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            //var result = await _guardCheckInOutService.GuardCheckIn(truckCheckIn.BookingId, truckCheckIn.LicensePlate.Replace("-", ""), truckCheckIn.CheckInDateTime, userName);

            var result = await _interfaceService.ImportPO(pos);

            return OkResponse(result);
        }

    }
}
