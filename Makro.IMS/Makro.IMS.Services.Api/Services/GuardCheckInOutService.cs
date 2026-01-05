using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
//using Makro.IMS.Services.Api.Models;
using Makro.IMS.Services.Api.Sieve;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Makro.IMS.Services.Api.Services
{
    public class GuardCheckInOutService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public GuardCheckInOutService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public GuardCheckInOutService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        #region +++ Guard Check In +++

        public async Task<BookingHeaderDto> GetBookingGuardCheckIn(string bookingId)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingId);

            if (bookingHeader == null)
            {
                throw new Exception("Incorrect Booking Id" + "\r\n" + "หมายเลขนัดหมายผิด");
            }

            List<string> statuses = new List<string>();

            List<string> truckStatus = new List<string>();
            truckStatus.Add("CHECKIN");
            truckStatus.Add("QUEUE");
            truckStatus.Add("CALLTRUCK");
            truckStatus.Add("ONDOCK");
            truckStatus.Add("UNLOADING");
            truckStatus.Add("UNLOADED");
            truckStatus.Add("LEAVEDOOR");
            truckStatus.Add("SUBMITDOC");

            statuses.Add("CHECKIN");
            statuses.Add("QUEUE");
            statuses.Add("CALLTRUCK");
            statuses.Add("ONDOCK");
            statuses.Add("UNLOADING");
            statuses.Add("UNLOADED");
            statuses.Add("LEAVEDOOR");
            statuses.Add("SUBMITDOC");
            statuses.Add("WATITINGDOC");
            statuses.Add("CHECKOUT");
            statuses.Add("COMPLETED");

            if (bookingHeader.Status.ToLower() == "new")
            {
                //throw new Exception("This booking on the approve process");
                throw new Exception("Booking Id on the approve process" + "\r\n" + "หมายเลขนัดหมายยังมาในขั้นตอนอนุมัติ");
            }
            

            bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
            bookingHeaderDto.SupCode = bookingHeader.SupCode;
            bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
            bookingHeaderDto.BookingId = bookingHeader.BookingId;
            bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
            bookingHeaderDto.DockDoor = bookingHeader.InternalDoorId.Value == 0 ? "-" : unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
            bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
            bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;
            bookingHeaderDto.Status = bookingHeader.Status;
            bookingHeaderDto.MerchType = bookingHeader.MerchType;
            bookingHeaderDto.CompanyCode = bookingHeader.CompanyCode;

            truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

            // get booking detail
            bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey).ToList();

            // get booking truck
            bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingHeader.InternalHeaderKey).ToList();

            var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey).ToList();

            if (statuses.Contains(bookingHeader.Status.ToUpper()))
            {
                if (bookingTruckCheckIns.Count(x => x.Status.ToUpper() == "INTRANSIT") > 0 ){

                }
                else {
                    throw new Exception("Booking Id already check-in" + "\r\n" + "หมายเลขนัดหมายถูก check-in แล้ว");
                }
            }

            bookingTruckCheckIns.ForEach((truckCheckIn) =>
            {
                BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckCheckIn.InternalTruckCheckInId);
                checkInDto.InternalTruckId = Convert.ToInt32(truckCheckIn.InternalTruckId);
                checkInDto.DriverName = truckCheckIn.DriverName;
                checkInDto.LicensePlate = truckCheckIn.LicensePlate;
                checkInDto.LicensePlate2 = truckCheckIn.LicensePlate2;
                checkInDto.TelNo = truckCheckIn.TelNo;
                checkInDto.LineId = truckCheckIn.LineId;
                checkInDto.TruckType = truckMasters.FirstOrDefault(x => x.InternalTruckId == truckCheckIn.InternalTruckId).TruckCode;
                checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();
                checkInDto.ArrivedTime = truckCheckIn.ArrivedTime;
                checkInDto.QueueSeq = truckCheckIn.QueueSeq;
                checkInDto.AssignQueueTime = truckCheckIn.AssignQueueTime;
                checkInDto.OndockTime = truckCheckIn.OndockTime;
                checkInDto.StartUnloadTime = truckCheckIn.StartUnloadTime;
                checkInDto.FinishUnloadTime = truckCheckIn.FinishUnloadTime;
                checkInDto.DepartureTime = truckCheckIn.DepartureTime;
                checkInDto.CheckInTime = truckCheckIn.CheckInTime;
                checkInDto.Status = truckCheckIn.Status;
                checkInDto.SubmitdocTime = truckCheckIn.SubmitdocTime;
                checkInDto.CalltruckTime = truckCheckIn.CalltruckTime;
                checkInDto.DoccheckTime = truckCheckIn.DoccheckTime;
                checkInDto.CheckoutTime = truckCheckIn.CheckoutTime;
                checkInDto.DateTimeStamp = truckCheckIn.DateTimeStamp;

                var detailCheckIns = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(truckCheckIn.InternalTruckCheckInId)).ToList();

                checkInDto.BookingTruckCheckInDetails = detailCheckIns;

                bookingCheckIns.Add(checkInDto);
            });

            var totalTrucks = bookingTrucks//.Select(x => new { truckId = x.InternalTruckId, totalTuck = x.TotalTruck})
                .GroupBy(x => x.InternalTruckId)
                .Select(s => new
                {
                    truckId = s.Key,
                    totalTruck = s.Sum(x => x.TotalTruck)
                });

            string strTotalTruck = "";

            foreach (var item in totalTrucks)
            {
                strTotalTruck += " " + truckMasters.Find(x => x.InternalTruckId == item.truckId).TruckName + " [" + item.totalTruck.ToString() + "]";
            }

            bookingHeaderDto.BookingDetails = bookingDetails;
            bookingHeaderDto.BookingTrucks = bookingTrucks;
            bookingHeaderDto.TotalTruck = strTotalTruck;
            bookingHeaderDto.BookingCheckIns = bookingCheckIns;

            return bookingHeaderDto;
        }

        public async Task<bool> SaveGuardCheckIn(GuardCheckInOutDto data)
        {
            // Check duplicate
            var status = new List<string>();
            status.Add("INTRANSIT");
            status.Add("CHECKIN");
            status.Add("CHECKIN_PROCESS");


            var bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(data.BookingId);
            
            if(bookingHeader != null && status.Contains(bookingHeader.Status))
            {
                // stamp guard check in time.
                bookingHeader.Status = "CHECKIN";
                bookingHeader.UserStamp = data.UserName;
                bookingHeader.ModDate = DateTime.Now;

                var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey);

                foreach (var item in bookingDtls)
                {
                    item.Status = "CHECKIN";
                    item.CheckIn = DateTime.Now;

                    await Task.Run(() => unitOfWork.BookingDetailRepository.Update(item));
                }

                var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                // stamp arrived time
                foreach (var item in bookingTrucks)
                {
                    item.ArrivedTime = DateTime.Now;
                    item.DateTimeStamp = DateTime.Now;

                    await Task.Run(() => unitOfWork.BookingTruckCheckInRepository.Update(item));
                    
                }

                unitOfWork.Save();

            }
            else
            {
                throw new Exception("Gate pass cannot check in, Please contact data entry" + "\r\n" + "ไม่สามารถทำการยืนยันการเข้าคลังได้, กรุณาติดต่อ Data entry");
            }
            return true;
        }

        public async Task<bool> SaveGuardCheckInByTruck(GuardCheckInOutDto data)
        {
            // Check duplicate
            var status = new List<string>();
            status.Add("INTRANSIT");
            status.Add("CHECKIN");
            status.Add("CHECKIN_PROCESS");


            var bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(data.BookingId);
            var bookingTruckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(data.InternalTruckCheckInId);

            if (bookingHeader != null && status.Contains(bookingHeader.Status))
            {

                bookingTruckCheckIn.ArrivedTime = DateTime.Now;
                bookingTruckCheckIn.UserStamp = data.UserName;
                bookingTruckCheckIn.Status = "CHECKIN";
                bookingTruckCheckIn.DateTimeStamp = DateTime.Now;
                
                unitOfWork.BookingTruckCheckInRepository.Update(bookingTruckCheckIn);


                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CHECKIN";
                truckLog.Remark = "";
                truckLog.UserStamp = data.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = bookingTruckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);
                

                unitOfWork.Save();

                var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                if(bookingTrucks.Count(x=>x.Status == "INTRANSIT") > 0)
                {
                    // stamp guard check in time.
                    //bookingHeader.Status = "CHECKIN_PROCESS";
                    //bookingHeader.UserStamp = data.UserName;
                    //bookingHeader.ModDate = DateTime.Now;
                }
                else
                {
                    // stamp guard check in time.
                    bookingHeader.Status = "CHECKIN";
                    bookingHeader.UserStamp = data.UserName;
                    bookingHeader.ModDate = DateTime.Now;
                }

                var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey);

                var bookingTruckDetails = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(data.InternalTruckCheckInId);

                foreach (var item in bookingDtls)
                {
                    if (bookingTruckDetails.Count(x => x.PoNbr == item.PoNbr) > 0)
                    {
                        item.Status = "CHECKIN";
                        item.CheckIn = DateTime.Now;

                        await Task.Run(() => unitOfWork.BookingDetailRepository.Update(item));
                    }
                }

                //var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                //// stamp arrived time
                //foreach (var item in bookingTrucks)
                //{
                //    item.ArrivedTime = DateTime.Now;

                //    await Task.Run(() => unitOfWork.BookingTruckCheckInRepository.Update(item));

                //}

                unitOfWork.Save();

            }
            else
            {
                throw new Exception("Gate pass cannot check in, Please contact data entry" + "\r\n" + "ไม่สามารถทำการยืนยันการเข้าคลังได้, กรุณาติดต่อ Data entry");
            }
            return true;
        }

        public async Task<bool> GuardCheckIn(string bookingId,string licenplate,DateTime checkinTime,string userName)
        {
            // Check duplicate
            var status = new List<string>();
            status.Add("INTRANSIT");
            status.Add("CHECKIN");
            status.Add("CHECKIN_PROCESS");

            var bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingId);
            var bookingTruckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey).FirstOrDefault(x=>x.LicensePlate.Replace("-","") == licenplate);

            if (bookingHeader != null && status.Contains(bookingHeader.Status))
            {

                bookingTruckCheckIn.ArrivedTime = DateTime.Now;
                bookingTruckCheckIn.UserStamp = userName;
                bookingTruckCheckIn.Status = "CHECKIN";
                bookingTruckCheckIn.DateTimeStamp = DateTime.Now;

                unitOfWork.BookingTruckCheckInRepository.Update(bookingTruckCheckIn);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CHECKIN";
                truckLog.Remark = "";
                truckLog.UserStamp = userName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = bookingTruckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

                var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                if (bookingTrucks.Count(x => x.Status == "INTRANSIT") > 0)
                {
                    // stamp guard check in time.
                    bookingHeader.Status = "CHECKIN_PROCESS";
                    bookingHeader.UserStamp = userName;
                    bookingHeader.ModDate = DateTime.Now;
                }
                else
                {
                    // stamp guard check in time.
                    bookingHeader.Status = "CHECKIN";
                    bookingHeader.UserStamp = userName;
                    bookingHeader.ModDate = DateTime.Now;
                }

                var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey);

                var bookingTruckDetails = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(bookingTruckCheckIn.InternalTruckCheckInId));

                foreach (var item in bookingDtls)
                {
                    if (bookingTruckDetails.Count(x => x.PoNbr == item.PoNbr) > 0)
                    {
                        item.Status = "CHECKIN";
                        item.CheckIn = DateTime.Now;

                        await Task.Run(() => unitOfWork.BookingDetailRepository.Update(item));
                    }
                }

                unitOfWork.Save();

            }
            else
            {
                return false;
                //throw new Exception("Gate pass cannot check in, Please contact data entry" + "\r\n" + "ไม่สามารถทำการยืนยันการเข้าคลังได้, กรุณาติดต่อ Data entry");
            }
            return true;
        }



        #endregion

        #region +++ Guard Check Out +++

        public async Task<BookingHeaderDto> GetBookingGuardCheckOut(string bookingId)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingId);

            if (bookingHeader == null)
            {
                throw new Exception("Incorrect Booking Id" + "\r\n" + "หมายเลขนัดหมายผิด");
            }

            List<string> statuses = new List<string>();

            //statuses.Add(new string["CHECKIN", "QUEUE", "CALLTRUCK", "ONDOCK", "UNLOADING", "UNLOADED", "LEAVEDOOR", "SUBMITDOC"])

            List<string> truckStatus = new List<string>();
            truckStatus.Add("CHECKIN");
            truckStatus.Add("QUEUE");
            truckStatus.Add("CALLTRUCK");
            truckStatus.Add("ONDOCK");
            truckStatus.Add("UNLOADING");
            truckStatus.Add("UNLOADED");
            truckStatus.Add("LEAVEDOOR");
            truckStatus.Add("SUBMITDOC");

            statuses.Add("NEW");
            //statuses.Add("INTRANSIT");

            if (statuses.Contains(bookingHeader.Status))
            {
                //throw new Exception("This booking on the approve process");
                throw new Exception("Booking Id not yet check in" + "\r\n" + "หมายเลขนัดหมายยังมาไม่ถึงคลัง");
            }

            if (bookingHeader.Status.ToLower() == "checkout")
            {
                throw new Exception("Booking Id already check-out" + "\r\n" + "หมายเลขนัดหมายถูก check-out แล้ว");
            }

            bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
            bookingHeaderDto.SupCode = bookingHeader.SupCode;
            bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
            bookingHeaderDto.BookingId = bookingHeader.BookingId;
            bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
            bookingHeaderDto.DockDoor = bookingHeader.InternalDoorId.Value == 0 ? "-" : unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
            bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
            bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;
            bookingHeaderDto.Status = bookingHeader.Status;
            bookingHeaderDto.MerchType = bookingHeader.MerchType;
            bookingHeaderDto.CompanyCode = bookingHeader.CompanyCode;

            truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

            // get booking detail
            bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey).ToList();

            // get booking truck
            bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingHeader.InternalHeaderKey).ToList();

            var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey).ToList();

            bookingTruckCheckIns.ForEach((truckCheckIn) =>
            {
                BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckCheckIn.InternalTruckCheckInId);
                checkInDto.InternalTruckId = Convert.ToInt32(truckCheckIn.InternalTruckId);
                checkInDto.DriverName = truckCheckIn.DriverName;
                checkInDto.LicensePlate = truckCheckIn.LicensePlate;
                checkInDto.TelNo = truckCheckIn.TelNo;
                checkInDto.LineId = truckCheckIn.LineId;
                checkInDto.TruckType = truckMasters.FirstOrDefault(x => x.InternalTruckId == truckCheckIn.InternalTruckId).TruckCode;
                checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();
                checkInDto.ArrivedTime = truckCheckIn.ArrivedTime;
                checkInDto.QueueSeq = truckCheckIn.QueueSeq;
                checkInDto.AssignQueueTime = truckCheckIn.AssignQueueTime;
                checkInDto.OndockTime = truckCheckIn.OndockTime;
                checkInDto.StartUnloadTime = truckCheckIn.StartUnloadTime;
                checkInDto.FinishUnloadTime = truckCheckIn.FinishUnloadTime;
                checkInDto.DepartureTime = truckCheckIn.DepartureTime;
                checkInDto.CheckInTime = truckCheckIn.CheckInTime;
                checkInDto.Status = truckCheckIn.Status;
                checkInDto.SubmitdocTime = truckCheckIn.SubmitdocTime;
                checkInDto.CalltruckTime = truckCheckIn.CalltruckTime;
                checkInDto.DoccheckTime = truckCheckIn.DoccheckTime;
                checkInDto.CheckoutTime = truckCheckIn.CheckoutTime;
                checkInDto.DateTimeStamp = truckCheckIn.DateTimeStamp;

                var detailCheckIns = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(truckCheckIn.InternalTruckCheckInId)).ToList();

                checkInDto.BookingTruckCheckInDetails = detailCheckIns;

                bookingCheckIns.Add(checkInDto);
            });

            var totalTrucks = bookingTrucks//.Select(x => new { truckId = x.InternalTruckId, totalTuck = x.TotalTruck})
                .GroupBy(x => x.InternalTruckId)
                .Select(s => new
                {
                    truckId = s.Key,
                    totalTruck = s.Sum(x => x.TotalTruck)
                });

            string strTotalTruck = "";

            foreach (var item in totalTrucks)
            {
                strTotalTruck += " " + truckMasters.Find(x => x.InternalTruckId == item.truckId).TruckName + " [" + item.totalTruck.ToString() + "]";
            }

            bookingHeaderDto.BookingDetails = bookingDetails;
            bookingHeaderDto.BookingTrucks = bookingTrucks;
            bookingHeaderDto.TotalTruck = strTotalTruck;
            bookingHeaderDto.BookingCheckIns = bookingCheckIns;

            return bookingHeaderDto;
        }


        public async Task<bool> GuardCheckOut(string licenseplate, DateTime checkoutTime, string userName)
        {
            // Check duplicate
            var status = new List<string>();
            status.Add("COMPLETED");

            // Get truck checkin by licenseplate
            var truckCheckIns = unitOfWork.BookingHeaderRepository.GetTruckCheckoutByLicensePlate(licenseplate);

            foreach (var data in truckCheckIns)
            {
                var bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(data.BookingId);
                var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                var bookingTruckCheckIn = bookingTruckCheckIns.FirstOrDefault(x => x.LicensePlate.Replace("-", "").Replace(" ", "") == licenseplate.Replace(" ", "")
                || x.LicensePlate2.Replace("-", "").Replace(" ", "") == licenseplate.Replace(" ", "")
                );

                if (bookingHeader != null && bookingTruckCheckIn.Status.ToUpper() != "INTRANSIT")
                {
                    var curDoor = unitOfWork.DoorRepository.GetDoors().FirstOrDefault(x => x.BookingHeaderKey == bookingTruckCheckIn.InternalTruckCheckInId);

                    if (curDoor != null)
                    {
                        curDoor.BookingHeaderKey = null;
                        unitOfWork.DoorRepository.Update(curDoor);
                    }


                    bookingTruckCheckIn.CheckoutTime = checkoutTime.ToLocalTime();
                    bookingTruckCheckIn.UserStamp = userName;
                    bookingTruckCheckIn.Status = "CHECKOUT";
                    bookingTruckCheckIn.DateTimeStamp = DateTime.Now;

                    unitOfWork.BookingTruckCheckInRepository.Update(bookingTruckCheckIn);

                    // save log
                    BookingTruckLog truckLog = new BookingTruckLog();
                    truckLog.Action = "CHECKOUT";
                    truckLog.Remark = "";
                    truckLog.UserStamp = userName;
                    truckLog.DateTimeStamp = DateTime.Now;
                    truckLog.InternalTruckCheckInId = bookingTruckCheckIn.InternalTruckCheckInId;

                    unitOfWork.BookingTruckLogRepository.Add(truckLog);

                    unitOfWork.Save();

                    var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                    if (bookingTrucks.Count(x => x.Status != "CHECKOUT") > 0)
                    {
                        // stamp guard check in time.
                        //bookingHeader.Status = "CHECKOUT_PROCESS";
                        //bookingHeader.UserStamp = userName;
                        //bookingHeader.ModDate = DateTime.Now;
                    }
                    else
                    {
                        // stamp guard check in time.
                        bookingHeader.Status = "CHECKOUT";
                        bookingHeader.UserStamp = userName;
                        bookingHeader.ModDate = DateTime.Now;
                    }

                    var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey);

                    var bookingTruckDetails = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(bookingTruckCheckIn.InternalTruckCheckInId));

                    foreach (var item in bookingDtls)
                    {
                        if (bookingTruckDetails.Count(x => x.PoNbr == item.PoNbr) > 0)
                        {
                            item.Status = "CHECKOUT";
                            item.CheckOut = checkoutTime.ToLocalTime();

                            await Task.Run(() => unitOfWork.BookingDetailRepository.Update(item));
                        }
                    }


                    unitOfWork.Save();

                }
                else
                {
                    return false;
                    //throw new Exception("Gate pass cannot check out, Please contact data entry" + "\r\n" + "ไม่สามารถทำการยืนยันออกจากคลังได้, กรุณาติดต่อ Data entry");
                }
            }


            
            return true;
            
        }


        public async Task<bool> SaveGuardCheckOutByTruck(GuardCheckInOutDto data)
        {
            // Check duplicate
            var status = new List<string>();
            status.Add("COMPLETED");
            

            var bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(data.BookingId);
            var bookingTruckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(data.InternalTruckCheckInId);

            if (bookingHeader != null && bookingTruckCheckIn.Status.ToUpper() != "INTRANSIT")
            {
                var curDoor = unitOfWork.DoorRepository.GetDoors().FirstOrDefault(x => x.BookingHeaderKey == bookingTruckCheckIn.InternalTruckCheckInId);

                if(curDoor != null)
                {
                    curDoor.BookingHeaderKey = null;
                    unitOfWork.DoorRepository.Update(curDoor);
                }


                bookingTruckCheckIn.CheckoutTime = DateTime.Now;
                bookingTruckCheckIn.UserStamp = data.UserName;
                bookingTruckCheckIn.Status = "CHECKOUT";
                bookingTruckCheckIn.DateTimeStamp = DateTime.Now;

                unitOfWork.BookingTruckCheckInRepository.Update(bookingTruckCheckIn);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CHECKOUT";
                truckLog.Remark = "";
                truckLog.UserStamp = data.UserName;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = bookingTruckCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

                unitOfWork.Save();

                var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                if (bookingTrucks.Count(x => x.Status != "CHECKOUT") > 0)
                {
                    // stamp guard check in time.
                    //bookingHeader.Status = "CHECKOUT_PROCESS";
                    //bookingHeader.UserStamp = data.UserName;
                    //bookingHeader.ModDate = DateTime.Now;
                }
                else
                {
                    // stamp guard check in time.
                    bookingHeader.Status = "CHECKOUT";
                    bookingHeader.UserStamp = data.UserName;
                    bookingHeader.ModDate = DateTime.Now;
                }

                var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey);

                var bookingTruckDetails = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(data.InternalTruckCheckInId);

                foreach (var item in bookingDtls)
                {
                    if (bookingTruckDetails.Count(x => x.PoNbr == item.PoNbr) > 0)
                    {
                        item.Status = "CHECKOUT";
                        item.CheckOut = DateTime.Now;

                        await Task.Run(() => unitOfWork.BookingDetailRepository.Update(item));
                    }
                }

                //var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeader.InternalHeaderKey);

                //// stamp arrived time
                //foreach (var item in bookingTrucks)
                //{
                //    item.ArrivedTime = DateTime.Now;

                //    await Task.Run(() => unitOfWork.BookingTruckCheckInRepository.Update(item));

                //}

                unitOfWork.Save();

            }
            else
            {
                throw new Exception("Gate pass cannot check out, Please contact data entry" + "\r\n" + "ไม่สามารถทำการยืนยันออกจากคลังได้, กรุณาติดต่อ Data entry");
            }
            return true;
        }


        #endregion

        #region ### Document Check In - Out ###

        public async Task<List<BookingCheckInDto>> DocumentCheckInPo(CheckInDto checkIn)
        {

            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByPoNo(checkIn.PoNbr);
            PoList po = new PoList();

            if (bookingDtls == null || bookingDtls.Count == 0)
            {
                // get po on ims
                po = unitOfWork.PoListRepository.GetPoByPoNo(checkIn.PoNbr, checkIn.SupCode, "88");

                if (po == null)
                {
                    throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
                }
                else
                {
                    // po not in booking
                    // add po in booking detail
                    BookingDetail bookingDetail = new BookingDetail();                                       
                }
            }

            for (int i = 0; i < bookingDtls.Count(); i++)
            {
                bookingDtls[i].Status = "DOCUMENT_CHECKIN";
                bookingDtls[i].DocumentCheckIn = DateTime.Now;
                bookingDtls[i].ModDate = DateTime.Now;
                bookingDtls[i].UserStamp = checkIn.UserStamp;

                unitOfWork.BookingDetailRepository.Update(bookingDtls[i]);
            }

            unitOfWork.Save();

            return await this.GetBookingCheckIn(checkIn.SupCode);

            //return null;
        }


        public async Task<List<BookingCheckInDto>> DocumentCheckInBooking(CheckInDto checkIn)
        {
            List<string> statuses = new List<string>();

            statuses.Add("CHECKIN");
            statuses.Add("CHECKIN_PROCESS");

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(checkIn.PoNbr);

            if (bookingHdr == null)
            {
                throw new Exception("Booking Id not found" + "\r\n" + "ไม่พบหมายเลขนัดหมาย");
            }

            if (!statuses.Contains(bookingHdr.Status))
            {
                throw new Exception("Booking Id cannot check in" + "\r\n" + "หมายเลขนัดหมายไม่สามารถทำการ check in ได้");
            }


            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHdr.InternalHeaderKey).ToList();

            if (bookingDtls == null || bookingDtls.Count == 0)
            {
                throw new Exception("Not found PO in this booking id" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อในการนัดหมาย");
            }

            for (int i = 0; i < bookingDtls.Count(); i++)
            {
                bookingDtls[i].Status = "DOCUMENT_CHECKIN";
                bookingDtls[i].DocumentCheckIn = DateTime.Now;
                bookingDtls[i].ModDate = DateTime.Now;
                bookingDtls[i].UserStamp = checkIn.UserStamp;

                unitOfWork.BookingDetailRepository.Update(bookingDtls[i]);
            }

            bookingHdr.Status = "CHECKIN";
            bookingHdr.UserStamp = checkIn.UserStamp;
            bookingHdr.ModDate = DateTime.Now;

            unitOfWork.BookingHeaderRepository.Update(bookingHdr);

            unitOfWork.Save();

            return await this.GetBookingCheckIn(checkIn.SupCode);

            //return null;
        }


        public async Task<List<BookingCheckInDto>> GetBookingCheckIn(string supCode)
        {
            var sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(supCode);

            var result = unitOfWork.PoListRepository.GetBookingDocumentCheckIn(sup.InternalGroupId.Value);
            return result;
        }

        #endregion

        #region +++ Manual booking +++

        public async Task<List<string>> CreateManualBooking(ManualBookingDto manualBooking)
        {
            Infra.Data.Models.BookingKey bookingKey = new Infra.Data.Models.BookingKey();
            Infra.Data.Models.Warehouse warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(manualBooking.WarehouseCode);

            var bookingKeyId = unitOfWork.PoListRepository.GetBookingKey();

            unitOfWork.Save();

            bookingKey.InternalKeyId = bookingKeyId;
            bookingKey.BookingDate = manualBooking.BookingDate.ToLocalTime().Date;
            bookingKey.CreateDate = DateTime.Now;
            bookingKey.Active = true;
            bookingKey.CompanyCode = warehouse.CompanyCode;
            bookingKey.UserStamp = manualBooking.UserName;
            bookingKey.ModDate = DateTime.Now;            

            var result = await Task.Run<bool>(() => unitOfWork.BookingKeyRepository.Add(bookingKey));

            unitOfWork.Save();

            string bookingId = "";

            // generate booking header id
            warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(manualBooking.WarehouseCode);

            if (warehouse.OnlineBookingIdRunning == null)
            {
                warehouse.OnlineBookingIdRunning = 1;
            }
            else
            {
                warehouse.OnlineBookingIdRunning += 1;
            }

            unitOfWork.WarehouseRepository.Update(warehouse);

            unitOfWork.Save();

            bookingId = "M" + warehouse.OnlineBookingIdPrefix + DateTime.Now.Date.ToString("MMdd")
                    + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');

            Infra.Data.Models.BookingHeader bookingHdr = new Infra.Data.Models.BookingHeader();
            bookingHdr.InternalHeaderKey = unitOfWork.PoListRepository.GetBookingHeaderKey();
            bookingHdr.InternalKeyId = bookingKey.InternalKeyId;
            bookingHdr.InternalSupGroupId = manualBooking.SubGroupId;
            bookingHdr.InternalDoorId = 0;
            bookingHdr.BookingId = bookingId;
            
            DateTime bookingStart = DateTime.Now;
            DateTime bookingEnd = DateTime.Now;

            DateTime checkInDateTime = DateTime.Now;

            if(DateTime.Now.Minute > 30)
            {
                bookingStart = bookingStart.AddMinutes(-1 * bookingStart.Minute);
                bookingStart = bookingStart.AddSeconds(-1 * bookingStart.Second);
                bookingStart = bookingStart.AddHours(1);
                bookingEnd = bookingStart.AddHours(1);
            }
            else
            {
                bookingStart = bookingStart.AddMinutes(-1 * bookingStart.Minute);
                bookingStart = bookingStart.AddSeconds(-1 * bookingStart.Second);
                bookingEnd = bookingStart.AddHours(1);
            }

            bookingHdr.BookingStart = bookingStart;
            bookingHdr.BookingEnd = bookingEnd;
            bookingHdr.FirstBookginStart = bookingStart;
            bookingHdr.FirstBookingEnd = bookingEnd;
            bookingHdr.ContactName = manualBooking.ContactName;
            bookingHdr.ContactEmail = manualBooking.ContactEmail;
            bookingHdr.ContactTel = manualBooking.ContactPhone;
            bookingHdr.WarehouseCode = manualBooking.WarehouseCode;
            bookingHdr.SupCode = manualBooking.SupCode;
            bookingHdr.SupName = manualBooking.SupName;
            bookingHdr.TotalPo = 0;
            bookingHdr.TotalQty = 0;
            bookingHdr.BackHaul = false;
            bookingHdr.Active = true;
            bookingHdr.Status = "CHECKIN";
            bookingHdr.UserStamp = manualBooking.UserName;
            bookingHdr.CreateDate = DateTime.Now;
            bookingHdr.Remark = "Manual";
            bookingHdr.ModDate = DateTime.Now;
            bookingHdr.CompanyCode = warehouse.CompanyCode;
            bookingHdr.MerchType = manualBooking.OperationType;
            

            bookingHdr.Postponed = false;

            unitOfWork.BookingHeaderRepository.Add(bookingHdr);

            //Infra.Data.Models.TruckMaster truck = unitOfWork.TruckMasterRepository.GetTrucks().FirstOrDefault(x => x.TruckCode == manualBooking.TruckType);

            Infra.Data.Models.BookingTruck bookingTruck = new Infra.Data.Models.BookingTruck();
            bookingTruck.InternalTruckId = Convert.ToInt32(manualBooking.TruckType);
            bookingTruck.TotalTruck = 1;
            bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

            unitOfWork.BookingTruckRepository.Add(bookingTruck);

            var internalTruckCheckIn = unitOfWork.PoListRepository.GetBookingTruckCheckInKey();

            unitOfWork.Save();

            Infra.Data.Models.BookingTruckCheckIn truckCheckIn = new Infra.Data.Models.BookingTruckCheckIn();
            truckCheckIn.InternalHeaderKey = bookingHdr.InternalHeaderKey;
            truckCheckIn.InternalTruckId = Convert.ToInt32(manualBooking.TruckType);
            truckCheckIn.InternalTruckCheckInId = internalTruckCheckIn;
            truckCheckIn.LicensePlate = manualBooking.LicensePlate;
            truckCheckIn.LicensePlate2 = manualBooking.LicensePlate2;
            truckCheckIn.DriverName = manualBooking.DriverName;
            truckCheckIn.TelNo = manualBooking.TelNo;
            truckCheckIn.Status = "CHECKIN";
            truckCheckIn.DateTimeStamp = DateTime.Now;
            truckCheckIn.UserStamp = manualBooking.UserName;
            truckCheckIn.LineId = manualBooking.LineNo;
            truckCheckIn.ArrivedTime = checkInDateTime;

            unitOfWork.BookingTruckCheckInRepository.AddManualBooking(truckCheckIn);


            Infra.Data.Models.BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.InternalTruckCheckInId = internalTruckCheckIn;
            truckLog.Action = "CHECKIN";
            truckLog.UserStamp = manualBooking.UserName;
            truckLog.Remark = "MANUAL BOOKING";
            truckLog.DateTimeStamp = checkInDateTime;

            unitOfWork.BookingTruckLogRepository.Add(truckLog);
            
            unitOfWork.Save();

            List<string> strResult = new List<string>();
            strResult.Add(warehouse.WarehouseCode + " : " + bookingId);

            return strResult;
        }

        #endregion


    }
}
