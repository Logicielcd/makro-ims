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
    
    public class SupplierGroupController : ApiControllerBase
    {
        private readonly SupplierGroupService _supplierGroupService;
        public SupplierGroupController(
            [FromServices] SupplierGroupService supplierGroupService
            )
        {
            _supplierGroupService = supplierGroupService;
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _supplierGroupService.GetSupplierGroupPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _supplierGroupService.GetSupplierGroup();

            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplierGroupService.GetById(id);

            return OkResponse(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Add(SupplierGroup supGroup)
        {            
            var result = await _supplierGroupService.Add(supGroup);

            return OkResponse(result);
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] SupplierGroup supGroup)
        {            
            var result = await _supplierGroupService.Update(supGroup);

            return OkResponse(result);
        }


        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] SupplierGroup supGroup)
        {
            var result = await _supplierGroupService.Remove(supGroup);

            return OkResponse(result);
        }

    }
}
