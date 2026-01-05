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
    
    public class TruckCheckOutController : ApiControllerBase
    {
        private readonly DriverService _driverService;
        private readonly GuardCheckInOutService _guardCheckInOutService;
        

        public TruckCheckOutController(
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

            var result = await _driverService.GetGatePassCheckoutByLicensePlate(licenseplate.Replace("-", ""));

            return OkResponse(result);
        }

        [HttpPost()]
        public async Task<IActionResult> TruckCheckOut([FromBody]TruckCheckOutDto truckCheckOut)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            var getPassList = await _driverService.GetGatePassCheckoutByLicensePlate(truckCheckOut.LicensePlate.Replace("-", ""));

            var result = await _guardCheckInOutService.GuardCheckOut(truckCheckOut.LicensePlate.Replace("-",""),truckCheckOut.CheckInDateTime,userName);

            return OkResponse(getPassList);
        }

    }
}
