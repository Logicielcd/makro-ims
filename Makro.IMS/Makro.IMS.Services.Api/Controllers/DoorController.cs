using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System.Text.Json.Nodes;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class DoorController : ApiControllerBase
    {
        private readonly DoorService _doorService;
        public DoorController(
            [FromServices] DoorService doorService
            )
        {
            _doorService = doorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _doorService.GetDoors();

            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _doorService.GetDoorPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet("warehouse/{warehouseCode}")]
        public async Task<IActionResult> GetByWarehouse(string warehouseCode)
        {
            var result = await _doorService.GetDoorsByWarehouse(warehouseCode);

            return OkResponse(result);
        }

        [HttpGet("warehouse/{warehouseCode}/{supCode}")]
        public async Task<IActionResult> GetByWarehouseSup(string warehouseCode,string supCode)
        {
            var result = await _doorService.GetDoorsBySupCode(warehouseCode,supCode);

            return OkResponse(result);
        }

        [HttpGet("operation/{warehouseCode}/{operationType}/{internalTruckCheckinId}")]
        public async Task<IActionResult> GetByWarehouseOperationType(string warehouseCode, string operationType,int internalTruckCheckinId)
        {
            var result = await _doorService.GetDoorsByOperationType(warehouseCode, operationType, internalTruckCheckinId);

            return OkResponse(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _doorService.GetById(id);

            return OkResponse(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Add(Door door)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;
            door.UserStamp = userName;            
            var result = await _doorService.AddDoor(door);

            return OkResponse(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Door door)
        {
            var result = await _doorService.Update(door);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] dynamic doorId)
        {
            dynamic objectDoor = Newtonsoft.Json.JsonConvert.DeserializeObject(doorId.ToString());
            int id = objectDoor.doorId;
            var result = await _doorService.Delete(id);            
            return OkResponse(result);
        }

    }
}
