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

    public class JobController : ApiControllerBase
    {
        private readonly JobService _jobService;
        public JobController([FromServices] JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _jobService.GetJobPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetJobAll()
        {
            var result = await _jobService.GetJobs();

            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _jobService.GetById(id);

            return OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobService.Add(job, user);

            return OkResponse(result);
        }

        [HttpPost("confirmAssign")]
        public async Task<IActionResult> ConfirmAssign(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobService.ConfirmAssignJob(job, user);

            return OkResponse(result);
        }

        [HttpPost("confirmStart")]
        public async Task<IActionResult> ConfirmStart(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobService.ConfirmStartJob(job, user);

            return OkResponse(result);
        }

        [HttpPost("confirmComplete")]
        public async Task<IActionResult> ConfirmComplete(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobService.ConfirmCompleteJob(job, user);

            return OkResponse(result);
        }

        [HttpPost("cancelJob")]
        public async Task<IActionResult> Cancel(Job job)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobService.CancelJob(job, user);

            return OkResponse(result);
        }

        [HttpPost("updateInDc")]
        public async Task<IActionResult> UpdateInDc([FromBody] ChangeLocationRequest changeDoor)
        {
            try
            {
                var user = this.User.Identities.FirstOrDefault().Name;
                var result = await _jobService.UpdateInDc(changeDoor, user);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("updateOutDc")]
        public async Task<IActionResult> UpdateOutDc(OutDCRequestDto trailer)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _jobService.UpdateOutDc(trailer, user);

            return OkResponse(result);
        }
    }
}
