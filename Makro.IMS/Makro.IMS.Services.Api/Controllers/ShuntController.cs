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

    public class ShuntController : ApiControllerBase
    {
        private readonly ShuntService _shuntService;
        public ShuntController([FromServices] ShuntService shuntService)
        {
            _shuntService = shuntService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _shuntService.GetShunts();

            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _shuntService.GetShuntPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _shuntService.GetById(id);

            return OkResponse(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _shuntService.GetByStatus(status);

            return OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Shunt shunt)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _shuntService.Add(shunt, user);

            return OkResponse(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Shunt shunt)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _shuntService.Update(shunt, user);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Shunt shunt)
        {
            var result = await _shuntService.Delete(shunt.Id);

            return OkResponse(result);
        }
    }
}
