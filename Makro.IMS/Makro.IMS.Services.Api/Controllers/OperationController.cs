using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Sieve.Models;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class OperationController : ApiControllerBase
    {
        private readonly OperationService _operationService;
        private readonly OperationTimeService _operationTimeService;
        private readonly OperationCapacityService _operationCapacityService;
        private readonly OperationFixSlotService _operationFixSlotService;

        public OperationController(
            [FromServices] OperationService operationService,
            [FromServices] OperationTimeService operationTimeService,
            [FromServices] OperationCapacityService operationCapacityService,
            [FromServices] OperationFixSlotService operationFixSlotService
            )
        {
            _operationService = operationService;
            _operationTimeService = operationTimeService;
            _operationCapacityService = operationCapacityService;
            _operationFixSlotService = operationFixSlotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _operationService.GetOperations();

            return OkResponse(result);
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _operationService.GetOperationPaged(sieveModel);
            return OkResponse(result);
        }

        [HttpGet("{warehouseCode}")]
        public async Task<IActionResult> GetByWarehouse(string warehouseCode)
        {
            var result = await _operationService.GetOperationsByWarehouse(warehouseCode);

            return OkResponse(result);
        }


        [HttpGet("{operationName}/{warehouseCode}")]
        public async Task<IActionResult> GetByNameAndWhse(string operationName,string warehouseCode)
        {
            var result = await _operationService.GetByNameAndWhse(operationName,warehouseCode);

            return OkResponse(result);
        }

        [HttpGet("operationtime/{operationName}/{warehouseCode}")]
        public async Task<IActionResult> GetOperationTimeByNameAndWhse(string operationName, string warehouseCode)
        {
            var result = await _operationTimeService.GetOperationTimesByWarehouseAndOperationType(warehouseCode, operationName);

            return OkResponse(result);
        }

        [HttpPost("delete/operationtime")]
        public async Task<IActionResult> DeleteOperationtime(OperationTime operationTime)
        {
            try
            {
                var result = await _operationTimeService.Delete(operationTime.Id);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> AddOperation(Operation operation)
        {
            try
            {
                var result = await _operationService.Add(operation);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("operationtime")]
        public async Task<IActionResult> Add(OperationTime operationTime)
        {
            try
            {
                var result = await _operationTimeService.Add(operationTime);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Operation operation)
        {
            var result = await _operationService.Update(operation);

            return OkResponse(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Operation operation)
        {
            var result = await _operationService.Delete(operation);            
            return OkResponse(result);
        }

        [HttpGet("operationcapacity/{operationName}/{warehouseCode}")]
        public async Task<IActionResult> GetOperationCapacityByNameAndWhse(string operationName, string warehouseCode)
        {
            var result = await _operationCapacityService.GetByNameAndWhse(operationName, warehouseCode);

            return OkResponse(result);
        }

        [HttpPost("operationcapacity")]
        public async Task<IActionResult> SaveOperationCapacity(OperationCapacity operationCapacity)
        {
            try
            {
                var result = await _operationCapacityService.Add(operationCapacity);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet("operationfixslot/{operationName}/{warehouseCode}")]
        public async Task<IActionResult> GetOperationFixSlotByNameAndWhse(string operationName, string warehouseCode)
        {
            var result = await _operationFixSlotService.GetOperationFixSlotByWhseAndOpType(warehouseCode, operationName);

            return OkResponse(result);
        }


        [HttpGet("operationfixslot/{operationName}/{warehouseCode}/{supGroupId}")]
        public async Task<IActionResult> GetOperationFixSlotByNameAndWhseAndSupGroup(string operationName, string warehouseCode,decimal supGroupId)
        {
            var result = await _operationFixSlotService.GetOperationFixSlotByWhseAndOpTypeAndSupGroupId(warehouseCode, operationName,supGroupId);

            return OkResponse(result);
        }

        [HttpPost("operationfixslot")]
        public async Task<IActionResult> SaveOperationFixSlot(OperationFixSlot operationFixSlot)
        {
            try
            {
                var result = await _operationFixSlotService.Add(operationFixSlot);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("delete/operationfixslot")]
        public async Task<IActionResult> DeleteOperationFixSlot(OperationFixSlot operationFixSlot)
        {
            var result = await _operationFixSlotService.Delete(operationFixSlot.Id);
            return OkResponse(result);
        }
    }
}
