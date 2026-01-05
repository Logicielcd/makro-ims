using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]

    public class YardController : ApiControllerBase
    {
        private readonly YardService _yardService;
        public YardController([FromServices] YardService yardService)
        {
            _yardService = yardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _yardService.GetYards();

            return OkResponse(result);
        }

        [HttpGet("yardMonitor")]
        public async Task<IActionResult> GetYardMonitor()
        {
            var result = await _yardService.GetYardMonitors();

            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _yardService.GetYardPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _yardService.GetById(id);

            return OkResponse(result);
        }

        [HttpGet("yardType/{yardType}")]
        public async Task<IActionResult> GetByYardType(string yardType)
        {
            var result = await _yardService.GetByYardType(yardType);

            return OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Yard yard)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _yardService.Add(yard, user);

            return OkResponse(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Yard yard)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _yardService.Update(yard, user);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Yard yard)
        {
            var result = await _yardService.Delete(yard.Id);

            return OkResponse(result);
        }

    }
}
