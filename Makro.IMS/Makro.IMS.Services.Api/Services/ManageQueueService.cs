using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Makro.IMS.Services.Api.Services
{
    public class ManageQueueService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public ManageQueueService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<QueueManageDto>> GetBookingQueue(string criteria)
        {
            List<QueueManageDto> result;

            result = unitOfWork.BookingHeaderRepository.GetBookingQueuePerTruck(criteria);

            return result;
        }

        public async Task<List<DoorQueueDto>> GetDoorQueue(string warehouseCode)
        {
            
            var result = unitOfWork.DoorRepository.GetDoorQueues(warehouseCode);

            return result.ToList();
        }

        public async Task<List<DoorQueueDto>> GetDoorQueueByOperationType(string warehouseCode,string operationType)
        {

            var result = unitOfWork.DoorRepository.GetDoorQueuesByOperationtype(warehouseCode,operationType);

            return result.ToList();
        }

        public async Task<bool> CreateQueue(QueueActionDto queueAction)
        {
            List<string> statusList = new List<string>();

            statusList.Add("CALLTRUCK");
            statusList.Add("ONDOCK");
            statusList.Add("QUEUE");
            statusList.Add("UNLOADING");
            statusList.Add("UNLOADED");
            statusList.Add("LEAVEDOOR");
            statusList.Add("SUBMITDOC");
            statusList.Add("WAITINGDOC");
            statusList.Add("CHECKOUT");

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);

            DateTime timeStamp = DateTime.Now;

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            if (!statusList.Contains(truckCheckIn.Status))
            {

                bookingHdr.MerchType = queueAction.Remark;
                //bookingHdr.Status = "QUEUE";                

                truckCheckIn.QueueSeq = Convert.ToDecimal(queueAction.QueueNo);
                truckCheckIn.AssignQueueTime = timeStamp;
                truckCheckIn.Status = "QUEUE";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                bookingHdr.Status = CheckStatus(queueAction, "QUEUE", bookingHdr.Status);
                bookingHdr.ModDate = timeStamp;
                unitOfWork.BookingHeaderRepository.Update(bookingHdr);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "QUEUE";
                truckLog.Remark = "";
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

            }

            return true;
        }

        public async Task<bool> OnDock(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);
            var door = unitOfWork.DoorRepository.GetDoorById(queueAction.InternalDoorId.Value);

            DateTime timeStamp = DateTime.Now;

          
            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);


            if (truckCheckIn.Status == "CALLTRUCK")
            {
                door.BookingHeaderKey = queueAction.InternalTruckCheckInId;


                truckCheckIn.OndockTime = timeStamp;
                truckCheckIn.Status = "ONDOCK";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                //bookingHdr.Status = "ONDOCK";
                bookingHdr.Status = CheckStatus(queueAction, "ONDOCK", bookingHdr.Status);
                bookingHdr.ModDate = timeStamp;
                unitOfWork.BookingHeaderRepository.Update(bookingHdr);
                unitOfWork.DoorRepository.Update(door);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "ONDOCK";
                truckLog.Remark = "";
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

            }

            return true;
        }

        public async Task<bool> CallTruck(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);
            var door = unitOfWork.DoorRepository.GetDoorById(queueAction.InternalDoorId.Value);

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);


            if (truckCheckIn.Status.ToUpper() == "QUEUE")
            {

                DateTime timeStamp = DateTime.Now;

                door.BookingHeaderKey = queueAction.InternalTruckCheckInId;
                
                //var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

                truckCheckIn.CalltruckTime = timeStamp;
                truckCheckIn.Status = "CALLTRUCK";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;
                truckCheckIn.Door = door.DoorName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                //bookingHdr.Status = "CALLTRUCK";
                bookingHdr.Status = CheckStatus(queueAction, "CALLTRUCK", bookingHdr.Status);
                bookingHdr.ModDate = timeStamp;
                bookingHdr.InternalDoorId = door.InternalDoorId;

                unitOfWork.BookingHeaderRepository.Update(bookingHdr);
                unitOfWork.DoorRepository.Update(door);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CALLTRUCK";
                truckLog.Remark = door.DoorName;
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();


                var whse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(bookingHdr.WarehouseCode);

                var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                var smsSetting = config.GetSection("SmsSetting");
                string smsMsg = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "CallTruckMsg").Value;


                // send sms
                SmsService smsService = new SmsService();
                SmsDto sms = new SmsDto();

                sms.TelNo = "66" + truckCheckIn.TelNo.Substring(1, 9);
                sms.SmsMessage = string.Format(smsMsg, truckCheckIn.LicensePlate, bookingHdr.BookingId, truckCheckIn.QueueSeq, door.DoorName, whse.PhoneNumber);

                var result = await smsService.SendSms(sms);

                // save log
                truckLog = new BookingTruckLog();
                truckLog.Action = "SEND SMS FOR CALL TRUCK";
                truckLog.Remark = result;
                truckLog.UserStamp = queueAction.UserName; 
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);
                unitOfWork.Save();

                
            }


            return true;
        }

        public async Task<bool> ChangeDoor(QueueActionDto queueAction)
        {

            List<string> statusList = new List<string>();
            
            statusList.Add("ONDOCK");
            statusList.Add("UNLOADING");
            statusList.Add("UNLOADED");
            
            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            if (statusList.Contains(truckCheckIn.Status))
            {

                //var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);
                var door = unitOfWork.DoorRepository.GetDoorById(queueAction.InternalDoorId.Value);
                var newDoor = unitOfWork.DoorRepository.GetDoorById(queueAction.NewInternalDoorId.Value);

                DateTime timeStamp = DateTime.Now;

                door.BookingHeaderKey = null;
                newDoor.BookingHeaderKey = queueAction.InternalTruckCheckInId;

                
                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CHANGE DOOR";
                truckLog.Remark = queueAction.Remark + ':' + newDoor.DoorName;
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                truckCheckIn.Door = newDoor.DoorName;
                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);

                //unitOfWork.BookingHeaderRepository.Update(bookingHdr);
                unitOfWork.DoorRepository.Update(door);
                unitOfWork.DoorRepository.Update(newDoor);

                unitOfWork.Save();

            }

            return true;
        }

        public async Task<bool> StartUnloading(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);

            DateTime timeStamp = DateTime.Now;

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            if (truckCheckIn.Status == "ONDOCK")
            {                
                truckCheckIn.StartUnloadTime = timeStamp;
                truckCheckIn.Status = "UNLOADING";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                //bookingHdr.Status = "UNLOADING";
                bookingHdr.Status = CheckStatus(queueAction, "UNLOADING", bookingHdr.Status);
                bookingHdr.ModDate = timeStamp;
                unitOfWork.BookingHeaderRepository.Update(bookingHdr);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "UNLOADING";
                truckLog.Remark = "";
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

            }

            return true;
        }

        public async Task<bool> FinishUnloading(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            if (truckCheckIn.Status.ToUpper() == "UNLOADING")
            {

                DateTime timeStamp = DateTime.Now;
                
                //var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

                truckCheckIn.FinishUnloadTime = timeStamp;
                truckCheckIn.Status = "UNLOADED";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                bookingHdr.Status = CheckStatus(queueAction, "UNLOADED", bookingHdr.Status);
                //bookingHdr.Status = "UNLOADED";
                bookingHdr.ModDate = timeStamp;
                unitOfWork.BookingHeaderRepository.Update(bookingHdr);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "UNLOADED";
                truckLog.Remark = "";
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

            }

            return true;
        }

        public async Task<bool> LeaveDoor(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);
 
            var door = unitOfWork.DoorRepository.GetDoorById(queueAction.InternalDoorId.Value);

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            DateTime timeStamp = DateTime.Now;

            if (truckCheckIn.Status.ToUpper() == "UNLOADED")
            {
                door.BookingHeaderKey = null;

               
                //var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);
                truckCheckIn.DepartureTime = timeStamp;
                truckCheckIn.Status = "LEAVEDOOR";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                // bookingHdr.Status = "LEAVEDOOR";
                bookingHdr.Status = CheckStatus(queueAction, "LEAVEDOOR", bookingHdr.Status);
                bookingHdr.ModDate = timeStamp;

                unitOfWork.BookingHeaderRepository.Update(bookingHdr);
                unitOfWork.DoorRepository.Update(door);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "LEAVEDOOR";
                truckLog.Remark = "";
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

            }

            return true;
        }

        public async Task<bool> SendDocument(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);
            
            DateTime timeStamp = DateTime.Now;

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            if (truckCheckIn.Status == "WAITINGDOC" || truckCheckIn.Status == "SUBMITDOC")
            {

                truckCheckIn.SubmitdocTime = timeStamp;
                truckCheckIn.Status = "SUBMITDOC";
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                bookingHdr.Status = CheckStatus(queueAction, "SUBMITDOC", bookingHdr.Status);
                unitOfWork.BookingHeaderRepository.Update(bookingHdr);
                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "SUBMITDOC";
                truckLog.Remark = "";
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();
            }
                return true;
            
        }

        public async Task<bool> UnloadFinish(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);

            DateTime timeStamp = DateTime.Now;

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            if (truckCheckIn.Status.ToUpper() == "LEAVEDOOR")
            {
                truckCheckIn.WaitingDocumentTime = timeStamp;
                truckCheckIn.WaitingDocument = queueAction.Status;
                truckCheckIn.Remark = queueAction.Remark;
                truckCheckIn.DateTimeStamp = timeStamp;
                truckCheckIn.UserStamp = queueAction.UserName;

                if (queueAction.Status == "Y")
                {
                    truckCheckIn.Status = "WAITINGDOC";                    
                }
                else
                {                    
                    truckCheckIn.Status = "SUBMITDOC";
                    truckCheckIn.SubmitdocTime = timeStamp;
                }

                unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
                unitOfWork.Save();

                if (queueAction.Status == "Y")
                {                 
                    bookingHdr.Status = CheckStatus(queueAction, "WAITINGDOC", bookingHdr.Status);
                }
                else
                {
                    bookingHdr.Status = CheckStatus(queueAction, "SUBMITDOC", bookingHdr.Status);                    
                }

                unitOfWork.BookingHeaderRepository.Update(bookingHdr);                

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = truckCheckIn.Status;
                truckLog.Remark = queueAction.Remark;
                truckLog.UserStamp = queueAction.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> SendSms(QueueManageDto queue,string userName)
        {
            
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var whse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(queue.WarehouseCode);

            var smsSetting = config.GetSection("SmsSetting");
            string smsMsg = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "ReceiveDocMsg").Value;

            // send sms
            SmsService smsService = new SmsService();
            SmsDto sms = new SmsDto();

            sms.TelNo = "66" + queue.TelNo.Substring(1, 9);
            sms.SmsMessage = string.Format(smsMsg, queue.LicensePlate, queue.BookingId,whse.PhoneNumber);

            var result = await smsService.SendSms(sms);

            // save log
            BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.Action = "SEND SMS FOR RECEIVED DOCUMENT";
            truckLog.Remark = result;
            truckLog.UserStamp = userName;
            truckLog.DateTimeStamp = DateTime.Now;
            truckLog.InternalTruckCheckInId = Convert.ToDecimal(queue.InternalTruckCheckInId);

            unitOfWork.BookingTruckLogRepository.Add(truckLog);

            unitOfWork.Save();


            return true;
        }

        public async Task<bool> CancelDoor(QueueActionDto queueAction)
        {
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(queueAction.InternalHeaderKey);
            var door = unitOfWork.DoorRepository.GetDoorById(queueAction.InternalDoorId.Value);
            //var newDoor = unitOfWork.DoorRepository.GetDoorById(queueAction.NewInternalDoorId.Value);

            DateTime timeStamp = DateTime.Now;
            
            door.BookingHeaderKey = null;

            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(queueAction.InternalTruckCheckInId.Value);

            truckCheckIn.CalltruckTime = null;
            truckCheckIn.OndockTime = null;
            truckCheckIn.StartUnloadTime = null;
            truckCheckIn.Status = "QUEUE";
            truckCheckIn.DateTimeStamp = DateTime.Now;
            truckCheckIn.Door = "";
           
            unitOfWork.BookingTruckCheckInRepository.Update(truckCheckIn);
            unitOfWork.Save();

            bookingHdr.Status = CheckStatus(queueAction, "QUEUE", bookingHdr.Status);
            //bookingHdr.Status = "QUEUE";
            bookingHdr.ModDate = timeStamp;
            bookingHdr.InternalDoorId = 0;

            unitOfWork.BookingHeaderRepository.Update(bookingHdr);

            unitOfWork.DoorRepository.Update(door);

            // save log
            BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.Action = "CANCEL DOOR";
            truckLog.Remark = queueAction.Remark + ":" + door.DoorName;
            truckLog.UserStamp = queueAction.UserName;
            truckLog.DateTimeStamp = DateTime.Now;
            truckLog.InternalTruckCheckInId = truckCheckIn.InternalTruckCheckInId;

            unitOfWork.BookingTruckLogRepository.Add(truckLog);

            unitOfWork.Save();

            return true;
        }

        public Dictionary<string,int> GetStatusPriority()
        {
            Dictionary<string, int> statusList = new Dictionary<string, int>();

            statusList.Add("CHECKIN", 1);
            statusList.Add("QUEUE", 2);
            statusList.Add("CALLTRUCK", 3);
            statusList.Add("ONDOCK",4);            
            statusList.Add("UNLOADING",5);
            statusList.Add("UNLOADED",6);
            statusList.Add("LEAVEDOOR",7);
            statusList.Add("WAITINGDOC", 8);
            statusList.Add("SUBMITDOC",9);            
            statusList.Add("CHECKOUT",10);

            return statusList;
        }

        public string CheckStatus(QueueActionDto queue,string newStatus,string bookingStatus)
        {
            var truckCheckInsInfo = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(queue.InternalHeaderKey).ToList();
           
            var truckStatuses = truckCheckInsInfo.GroupBy(x=>x.Status).Select(x => new { Status = x.Key, Count = x.Count() });

            var statusList = GetStatusPriority();

            var newStatusPriority = statusList.Where(x => x.Key == newStatus).FirstOrDefault().Value;
            var bookingStatusPriority = statusList.Where(x=>x.Key == bookingStatus).FirstOrDefault().Value;

           
                if (truckStatuses.Count() > 1)
                {
                    // get lessthan status priorty
                    var previousStatus = "";
                    var previousStatusPriority = 0;
                    foreach (var status in truckStatuses)
                    {
                        if(previousStatus == "")
                        {
                            previousStatus = status.Status;
                            previousStatusPriority = statusList.Where(x=>x.Key == status.Status).FirstOrDefault().Value;
                        }
                        else
                        {
                            if(previousStatusPriority > statusList.Where(x=>x.Key == status.Status).FirstOrDefault().Value)
                            {
                                previousStatus = status.Status;
                                previousStatusPriority = statusList.Where(x => x.Key == status.Status).FirstOrDefault().Value;
                            }
                        }
                    }

                    if (newStatusPriority > previousStatusPriority)
                    {
                        return previousStatus;
                    }
                    else
                    {
                        return newStatus;
                    }
                }
                else
                {
                    return newStatus;
                }
            

            //var currentStatusPriority = 0;

            //foreach (var status in truckStatuses)
            //{
            //    var statusPriority = statusList.Where(x=>x.Key == status.Status).FirstOrDefault().Value;

            //    if(currentStatusPriority == 0)
            //    {
            //        currentStatusPriority = statusPriority;
            //    }
            //    else if(statusPriority < currentStatusPriority) 
            //    {
            //        currentStatusPriority = statusPriority;
            //    }
            //}

            //if (currentStatusPriority < newStatusPriority)
            //{
            //    return statusList.Where(x => x.Value == currentStatusPriority).FirstOrDefault().Key.ToString();
            //}
            //return newStatus;
        }
    
        public async Task<BookingTruckCheckIn> GetBookingTruckCheckInAsync(int internalTruckCheckInId)
        {
            return unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(internalTruckCheckInId);
        }

        public async Task<bool> UpdateDriverInformation(BookingTruckCheckIn bookingTruckCheckIn)
        {
            var bookingTruck = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(Convert.ToInt32(bookingTruckCheckIn.InternalTruckCheckInId));

            bookingTruck.DriverName = bookingTruckCheckIn.DriverName;
            bookingTruck.TelNo = bookingTruckCheckIn.TelNo;

            unitOfWork.BookingTruckCheckInRepository.Update(bookingTruck);

            unitOfWork.Save();
            
            return true;
        }
    }
}
