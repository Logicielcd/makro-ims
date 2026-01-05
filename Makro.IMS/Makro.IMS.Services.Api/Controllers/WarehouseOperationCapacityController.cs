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
    
    public class WarehouseOperationCapacityController : ApiControllerBase
    {
        private readonly WarehouseOperationCapacityService _warehouseOperationCapacityService;
        public WarehouseOperationCapacityController(
            [FromServices] WarehouseOperationCapacityService warehouseOperationCapacityService
            )
        {
            _warehouseOperationCapacityService = warehouseOperationCapacityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warehouseOperationCapacityService.GetWarehouseCapacities();
            result.ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages([FromQuery] SieveModel sieveModel)
        {
            var result = await _warehouseOperationCapacityService.GetWarehouseCapacitiesPaged(sieveModel);
            result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _warehouseOperationCapacityService.GetById(id);
            result.BookingDate = result.BookingDate.Date;
            return OkResponse(result);
        }

        [HttpGet("{bookingdate}/{warehouse}")]
        public async Task<IActionResult> GetCapacity(DateTime bookingdate, string warehouse)
        {
            var result = await _warehouseOperationCapacityService.GetCapacityByBookingDate(bookingdate, warehouse);
            //result.ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WarehouseOperationCapacity warehouseCapacity)
        {
            var result = await _warehouseOperationCapacityService.CreateWarehouseOperationCapacity(warehouseCapacity);

            return OkResponse(result);
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] WarehouseOperationCapacity warehouseCapacity)
        {
            var result = await _warehouseOperationCapacityService.UpdateWarehouseOperationCapacity(warehouseCapacity);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] int warehouseCapacityId)
        {
            var result = await _warehouseOperationCapacityService.Delete(warehouseCapacityId);
            return OkResponse(result);
        }
    }
}
