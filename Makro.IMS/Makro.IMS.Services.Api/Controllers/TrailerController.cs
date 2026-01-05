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

    public class TrailerController : ApiControllerBase
    {
        private readonly TrailerService _trailerService;
        public TrailerController([FromServices] TrailerService trailerService)
        {
            _trailerService = trailerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _trailerService.GetTrailers();

            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _trailerService.GetTrailerPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _trailerService.GetById(id);

            return OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Trailer trailer)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _trailerService.Add(trailer, user);

            return OkResponse(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Trailer trailer)
        {
            var user = this.User.Identities.FirstOrDefault().Name;
            var result = await _trailerService.Update(trailer, user);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Trailer trailer)
        {
            var result = await _trailerService.Delete(trailer.Id);

            return OkResponse(result);
        }

    }
}
