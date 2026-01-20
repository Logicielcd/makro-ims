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
    
    public class WarehouseController : ApiControllerBase
    {
        private readonly WarehouseService _warehouseService;
        public WarehouseController(
            [FromServices] WarehouseService warehouseService
            )
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warehouseService.GetWarehouses();

            return OkResponse(result);
        }
        
        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _warehouseService.GetWarehousePaged(sieveModel);            
            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _warehouseService.GetById(id);

            return OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Warehouse warehouse)
        {
            var result = await _warehouseService.Add(warehouse);

            return OkResponse(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Warehouse warehouse)
        {
            var result = await _warehouseService.Update(warehouse);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Warehouse warehouse)
        {
            var result = await _warehouseService.Delete(warehouse.WarehouseCode);

            return OkResponse(result);
        }

    }
}
