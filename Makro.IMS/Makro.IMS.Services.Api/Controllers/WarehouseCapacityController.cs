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
    
    public class WarehouseCapacityController : ApiControllerBase
    {
        private readonly WarehouseCapacityService _warehouseCapacityService;
        public WarehouseCapacityController(
            [FromServices] WarehouseCapacityService warehouseCapacityService
            )
        {
            _warehouseCapacityService = warehouseCapacityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warehouseCapacityService.GetWarehouseCapacities();
            result.ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _warehouseCapacityService.GetWarehouseCapacitiesPaged(sieveModel);
            result.Results.ToList().ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _warehouseCapacityService.GetById(id);
            result.BookingDate = result.BookingDate.Date;
            return OkResponse(result);
        }

        [HttpGet("{bookingdate}/{warehouse}")]
        public async Task<IActionResult> GetCapacity(DateTime bookingdate, string warehouse)
        {
            var result = await _warehouseCapacityService.GetCapacityByBookingDate(bookingdate, warehouse);
            //result.ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }

        [HttpGet("{bookingdate}/{warehouse}/{operationtype}")]
        public async Task<IActionResult> GetCapacityByOperation(DateTime bookingdate, string warehouse,string operationtype)
        {
            var result = await _warehouseCapacityService.GetOperationCapacityByBookingDate(bookingdate, warehouse,operationtype);
            //result.ForEach(x => x.BookingDate = x.BookingDate.Date);
            return OkResponse(result);
        }



        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WarehouseCapacity warehouseCapacity)
        {
            var result = await _warehouseCapacityService.CreateWarehouseCapacity(warehouseCapacity);

            return OkResponse(result);
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] WarehouseCapacity warehouseCapacity)
        {
            var result = await _warehouseCapacityService.UpdateWarehouseCapacity(warehouseCapacity);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] int warehouseCapacityId)
        {
            var result = await _warehouseCapacityService.Delete(warehouseCapacityId);
            return OkResponse(result);
        }
    }
}
