using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using System;
using static System.Collections.Specialized.BitVector32;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class ManageQueueController : ApiControllerBase
    {
        private readonly ManageQueueService _manageQueueService;
        private readonly QueueSequenceService _queueSequenceService;
        private readonly PoCheckInOutService _poCheckInOutService;
        

        public ManageQueueController(
            [FromServices] ManageQueueService manageQueueService,
            [FromServices] QueueSequenceService queueSequenceService,
            [FromServices] PoCheckInOutService poCheckInOutService
            )
        {
            _manageQueueService = manageQueueService;
            _queueSequenceService = queueSequenceService;
            _poCheckInOutService = poCheckInOutService;
        }

        [HttpPost("managequeue")]
        public async Task<IActionResult> GetManageQueue([FromBody] QueueManageSearchDto search)
        {
            string criteria = "";

            if(search != null)
            {
                if (search.WarehouseCode != null)
                {
                    criteria += " and warehouse_code = '" + search.WarehouseCode + "' ";
                }
                if (search.OperationType != null && search.OperationType.ToUpper() != "ALL")
                {
                    criteria += " and merch_type = '" + search.OperationType + "' ";
                }
                if (search.Status != null && search.Status.ToUpper() != "ALL")
                {
                    criteria += " and btc.status = '" + search.Status + "' ";
                }
                if (search.StartDate != null && search.StartDate.Year > 2000)
                {
                    //criteria += " and to_char(booking_start,'YYYY-MM-DD') >= '" + search.StartDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";
                    criteria += " and to_char(date_time_stamp,'YYYY-MM-DD') >= '" + search.StartDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";
                }
                if (search.EndDate != null && search.EndDate.Year > 2000)
                {
                    //criteria += " and to_char(booking_end,'YYYY-MM-DD') <= '" + search.EndDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";
                    criteria += " and to_char(date_time_stamp,'YYYY-MM-DD') <= '" + search.EndDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";
                }
                if (!string.IsNullOrEmpty(search.BookingId))
                {
                    criteria += " and bh.booking_id = '" + search.BookingId + "'";
                }
                if (!string.IsNullOrEmpty(search.LicensePlate))
                {
                    criteria += " and btc.license_plate = '" + search.LicensePlate + "'";
                }
            }

            var result = await _manageQueueService.GetBookingQueue(criteria);
            
            return OkResponse(result);
        }

        [HttpPost("truckqueue")]
        public async Task<IActionResult> TruckQueue([FromBody] QueueManageSearchDto search)
        {
            //string criteria = " and queue_seq is not null and ondock_time is null ";
            string criteria = " and queue_seq is not null and calltruck_time is null and btc.status != 'CHECKOUT' ";

            if (search != null)
            {
                if (search.WarehouseCode != null)
                {
                    criteria += " and warehouse_code = '" + search.WarehouseCode + "' ";
                }
                if (search.OperationType != null && search.OperationType.ToUpper() != "ALL")
                {
                    var opts = search.OperationType.Split("|");

                    if (opts.Count() > 1)
                    {
                        criteria += " and merch_type in (";

                        for (int i = 0;i< opts.Count(); i++)
                        {
                            if (i == opts.Count() - 1)
                            {
                                criteria += $"'{opts[i]}'";
                            }
                            else
                            {
                                criteria += $"'{opts[i]}',";
                            }
                        }                       
                        criteria += ") ";
                    }
                    else
                    {
                        criteria += " and merch_type = '" + search.OperationType + "' ";
                    }
                }
                if (search.Status != null && search.Status.ToUpper() != "ALL")
                {
                    criteria += " and bh.status = '" + search.Status + "' ";
                }
                if (search.TruckType != null)
                {                   
                    criteria += " and replace('" + search.TruckType + "',' ','') like '%' || replace(tm.truck_code,' ','') || '%' ";
                }
                if (search.StartDate != null && search.StartDate.Year > 2000)
                {
                    //criteria += " and to_char(booking_start,'YYYY-MM-DD') >= '" + search.StartDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";
                    criteria += " and to_char(date_time_stamp,'YYYY-MM-DD') >= '" + search.StartDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";

                }
                if (search.EndDate != null && search.EndDate.Year > 2000)
                {
                    //criteria += " and to_char(booking_end,'YYYY-MM-DD') <= '" + search.EndDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";
                    criteria += " and to_char(date_time_stamp,'YYYY-MM-DD') <= '" + search.EndDate.ToLocalTime().ToString("yyyy-MM-dd") + "' ";

                }
                if (!string.IsNullOrEmpty(search.BookingId))
                {
                    criteria += " and bh.booking_id = '" + search.BookingId + "'";
                }
                if (!string.IsNullOrEmpty(search.LicensePlate))
                {
                    criteria += " and btc.license_plate = '" + search.LicensePlate + "'";
                }
            }

            var result = await _manageQueueService.GetBookingQueue(criteria);

            return OkResponse(result);
        }

        [HttpGet("queuedoor/{warehouseCode}")]
        public async Task<IActionResult> GetDoorQueueList(string warehouseCode)
        {
         
            var result = await _manageQueueService.GetDoorQueue(warehouseCode);

            return OkResponse(result);
        }

        [HttpGet("queuedoorbyoperation/{warehouseCode}/{operationType}")]
        public async Task<IActionResult> GetDoorQueueListByOperation(string warehouseCode,string operationType)
        {

            var result = await _manageQueueService.GetDoorQueueByOperationType(warehouseCode,operationType);

            return OkResponse(result);
        }

        [HttpGet("queuesequence/{warehouseCode}/{operationType}")]
        public async Task<IActionResult> GetQueueSequence(string warehouseCode,string operationType)
        {

            var result = await _queueSequenceService.GetQueueSequence(warehouseCode,operationType);

            return OkResponse(result);
        }

        [HttpPost("createqueue")]
        public async Task<IActionResult> CreateQueue([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;
            var result = await _manageQueueService.CreateQueue(action);

            return OkResponse(result);
        }

        [HttpPost("assigndoor")]
        public async Task<IActionResult> AssignDoor([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.CallTruck(action);

            return OkResponse(result);
        }

        [HttpPost("changedoor")]
        public async Task<IActionResult> ChangeDoor([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;
            var result = await _manageQueueService.ChangeDoor(action);

            return OkResponse(result);
        }

        [HttpPost("truckondoor")]
        public async Task<IActionResult> TruckOnDoor([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.OnDock(action);

            return OkResponse(result);
        }

        [HttpPost("startunloading")]
        public async Task<IActionResult> StartUnloading([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.StartUnloading(action);

            return OkResponse(result);
        }

        [HttpPost("finishunloading")]
        public async Task<IActionResult> FinishUnloading([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.FinishUnloading(action);

            return OkResponse(result);
        }

        [HttpPost("leavedoor")]
        public async Task<IActionResult> LeaveDoor([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.LeaveDoor(action);

            return OkResponse(result);
        }

        [HttpPost("senddocument")]
        public async Task<IActionResult> SendDocument([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.SendDocument(action);

            return OkResponse(result);
        }

        [HttpPost("unloadfinish")]
        public async Task<IActionResult> UnloadFinish([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.UnloadFinish(action);

            return OkResponse(result);
        }

        [HttpGet("pocheckin/{internaltruckcheckinid}")]
        public async Task<IActionResult> GetPoCheckIn(int internalTruckCheckInId)
        {
            var result = await _poCheckInOutService.GetPoCheckInByInternalTruckCheckInId(internalTruckCheckInId);

            return OkResponse(result);
        }

        [HttpPost("documentcheckinpo")]
        public async Task<IActionResult> DocumentCheckInPo([FromBody] CheckInDto checkIn)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;

                checkIn.UserStamp = userName;

                var result = await _poCheckInOutService.DocumentCheckInPo(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("documentcheckinponotinbooking")]
        public async Task<IActionResult> DocumentCheckInPoNotInBooking([FromBody] CheckInDto checkIn)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;

                checkIn.UserStamp = userName;

                var result = await _poCheckInOutService.PoNotInBooking(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("deletecheckinpo")]
        public async Task<IActionResult> DeleteCheckInPo([FromBody] CheckInDto checkIn)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;

                checkIn.UserStamp = userName;

                var result = await _poCheckInOutService.DeletePoCheckIn(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("deleteuploadpo")]
        public async Task<IActionResult> DeleteUnloadPo([FromBody] CheckInDto checkIn)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;

                checkIn.UserStamp = userName;

                var result = await _poCheckInOutService.DeletePoUnload(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpGet("pocheckout/{internaltruckcheckinid}")]
        public async Task<IActionResult> GetPoCheckOut(int internalTruckCheckInId)
        {

            var result = await _poCheckInOutService.GetPoCheckOutByInternalTruckCheckInId(internalTruckCheckInId);

            return OkResponse(result);
        }

        [HttpPost("documentcheckoutpo")]
        public async Task<IActionResult> DocumentCheckOutPo([FromBody] CheckInDto checkIn)
        {
            try
            {
                var result = await _poCheckInOutService.DocumentCheckOutPo(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("deletecheckoutpo")]
        public async Task<IActionResult> DeleteCheckOutPo([FromBody] CheckInDto checkIn)
        {
            try
            {
                var result = await _poCheckInOutService.DeletePoCheckOut(checkIn);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("sendsms")]
        public async Task<IActionResult> SendSms([FromBody] QueueManageDto queue)
        {
            try
            {
                string userName = this.User.Identities.FirstOrDefault().Name;

                var result = await _manageQueueService.SendSms(queue,userName);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("canceldoor")]
        public async Task<IActionResult> CancelDoor([FromBody] QueueActionDto action)
        {
            string userName = this.User.Identities.FirstOrDefault().Name;

            action.UserName = userName;

            var result = await _manageQueueService.CancelDoor(action);

            return OkResponse(result);
        }


        [HttpGet("bookingtruckcheckin/{internaltruckcheckinid}")]
        public async Task<IActionResult> GetBookingTruckCheckIn(int internalTruckCheckInId)
        {
            var result = await _manageQueueService.GetBookingTruckCheckInAsync(internalTruckCheckInId);

            return OkResponse(result);
        }

        [HttpPost("updatedriverinfo")]
        public async Task<IActionResult> UpdateDriverInfo([FromBody] BookingTruckCheckIn bookingTruckCheckIn)
        {
            var result = await _manageQueueService.UpdateDriverInformation(bookingTruckCheckIn);
            return OkResponse(result);
        }

    }
}
