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

    public class JobOnDockController : ApiControllerBase
    {
        private readonly JobOnDockService _jobOnDockService;
        public JobOnDockController([FromServices] JobOnDockService jobOnDockService)
        {
            _jobOnDockService = jobOnDockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobByYardOnDock()
        {
            var result = await _jobOnDockService.GetJobByYardOnDock();

            return OkResponse(result);
        }

        [HttpPost("changeDoor")]
        public async Task<IActionResult> ChangeDoor([FromBody] ChangeDoorRequestDto changeDoor)
        {
            try
            {
                var result = await _jobOnDockService.ChangeDoor(changeDoor);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("confirmLoading")]
        public async Task<IActionResult> ConfirmLoading(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobOnDockService.ConfirmLoading(job, user);

            return OkResponse(result);
        }

        [HttpPost("confirmLoaded")]
        public async Task<IActionResult> ConfirmLoaded(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobOnDockService.ConfirmLoaded(job, user);

            return OkResponse(result);
        }

        [HttpPost("confirmComplete")]
        public async Task<IActionResult> ConfirmComplete(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobOnDockService.ConfirmComplete(job, user);

            return OkResponse(result);
        }

        [HttpPost("createJobChangeTrailer")]
        public async Task<IActionResult> ChangeTrailerOnDockDto([FromBody] ChangeTrailerOnDockDto request)
        {
            try
            {
                var user = this.User.Identities.FirstOrDefault().Name;
                var result = await _jobOnDockService.ChangeTrailerOnDock(request, user);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
    }
}
