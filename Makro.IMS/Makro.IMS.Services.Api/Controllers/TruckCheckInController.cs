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
    
    public class TruckCheckInController : ApiControllerBase
    {
        private readonly DriverService _driverService;
        private readonly GuardCheckInOutService _guardCheckInOutService;
        

        public TruckCheckInController(
            [FromServices] DriverService driverService,
            [FromServices] GuardCheckInOutService guardCheckInOutService
            )
        {
            _driverService = driverService;    
            _guardCheckInOutService = guardCheckInOutService;
        }

        [HttpGet("{licenseplate}")]
        public async Task<IActionResult> GetGatePassByLicensePlate(string licenseplate) 
        {

            var result = await _driverService.GetGatePassByLicensePlate(licenseplate.Replace("-",""));

            return OkResponse(result);
        }

        [HttpPost()]
        public async Task<IActionResult> TruckCheckIn([FromBody]TruckCheckInDto truckCheckIn)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            var result = await _guardCheckInOutService.GuardCheckIn(truckCheckIn.BookingId,truckCheckIn.LicensePlate.Replace("-",""),truckCheckIn.CheckInDateTime,userName);

            return OkResponse(result);
        }

    }
}
