using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class ReportController : ApiControllerBase
    {        
        private readonly ReportService _reportService;
        public ReportController(
            [FromServices] ReportService reportService
            )
        {
            _reportService = reportService;
        }

        [HttpGet("gatepass/{internalHeaderKey}")]
        public async Task<IActionResult> GetGatePass(int internalHeaderKey)
        {
            try
            {
                
                var result = await _reportService.GetGatePass(internalHeaderKey);

                return OkResponse(result);
            }
            catch (Exception ex)                                 
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("bookingsummary/{warehouseCode}/{companyCode}")]
        public async Task<IActionResult> GetBookingSummary(string warehouseCode,string companyCode)
        {
            try
            {

                var result = await _reportService.GetSummaryBooking(warehouseCode,companyCode);

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet("truckstatus/{warehouseCode}/{companyCode}/{fromDate}/{toDate}")]
        public async Task<IActionResult> GetTruckStatus(string warehouseCode, string companyCode,DateTime fromDate,DateTime toDate)
        {
            try
            {
                var result = await _reportService.GetTruckStatus(warehouseCode, companyCode,fromDate.ToLocalTime(),toDate.ToLocalTime());

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("transactiontrack/{warehouseCode}/{companyCode}/{fromDate}/{toDate}")]
        public async Task<IActionResult> GetTransactionTrack(string warehouseCode, string companyCode,DateTime fromDate,DateTime toDate)
        {
            try
            {
                var result = await _reportService.GetTransactionTrack(warehouseCode, companyCode,fromDate.ToLocalTime().Date,toDate.ToLocalTime().Date);

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("slottimebooking/{warehouseCode}/{companyCode}/{fromDate}")]
        public async Task<IActionResult> GetSlottimeBooking(string warehouseCode, string companyCode, DateTime fromDate)
        {
            try
            {
                var result = await _reportService.GetSlottimeBooking(warehouseCode, companyCode, fromDate.ToLocalTime().Date);

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("slottimebookingactual/{warehouseCode}/{companyCode}/{fromDate}")]
        public async Task<IActionResult> GetSlottimeBookingActual(string warehouseCode, string companyCode, DateTime fromDate)
        {
            try
            {
                var result = await _reportService.GetSlottimeBookingActual(warehouseCode, companyCode, fromDate.ToLocalTime().Date);

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("slottimebookingpending/{warehouseCode}/{companyCode}/{fromDate}")]
        public async Task<IActionResult> GetSlottimeBookingPending(string warehouseCode, string companyCode, DateTime fromDate)
        {
            try
            {
                var result = await _reportService.GetSlottimeBookingPending(warehouseCode, companyCode, fromDate.ToLocalTime().Date);

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("dockdoorcontrol/{warehouseCode}/{doorrange}")]
        public async Task<IActionResult> GetDockDoorControl(string warehouseCode, string doorrange)
        {
            try
            {
                var result = await _reportService.GetDockDoorControl(warehouseCode, doorrange);

                return OkResponse(result!);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }
    }
}
