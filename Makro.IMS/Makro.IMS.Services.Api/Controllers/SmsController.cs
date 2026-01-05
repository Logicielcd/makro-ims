using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    
    public class SmsController : ApiControllerBase
    {
        private readonly SmsService _smsService;
        public SmsController(
            [FromServices] SmsService smsService
            )
        {
            _smsService = smsService;
        }

        [Authorize]
        [HttpPost("sendsms")]
        public async Task<IActionResult> SendSms(SmsDto sms)
        {
            var result = await _smsService.SendSms(sms);
            return OkResponse(result);
        }

    }
}
