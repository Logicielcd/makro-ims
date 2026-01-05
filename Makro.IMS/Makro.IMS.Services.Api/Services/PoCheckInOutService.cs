using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using System.Linq;

namespace Makro.IMS.Services.Api.Services
{
    public class PoCheckInOutService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public PoCheckInOutService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public PoCheckInOutService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        #region +++ Document Check In +++

        public async Task<List<PoCheckInOut>> GetPoCheckInByInternalTruckCheckInId(int internalTruckCheckInId)
        {
            return unitOfWork.PoCheckInOutRepository.GetPoCheckInOutsByInternalTruckCheckInId(internalTruckCheckInId).ToList();
        }

        public async Task<List<PoCheckInOut>> DocumentCheckInPo(CheckInDto checkIn)
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
                    //BookingDetail bookingDetail = new BookingDetail();
                    List<PoCheckInOut> list = new List<PoCheckInOut>();
                    list.Add(new PoCheckInOut
                    {
                        Id = 0,
                        PoNbr = checkIn.PoNbr,
                        Status = "NOTINBOOKING",
                        DateTimeStamp = DateTime.Now
                    });

                    return list;
                }
            }

            var poCheckin = unitOfWork.PoCheckInOutRepository.GetPoCheckIn(checkIn.PoNbr);

            if(poCheckin != null)
            {
                throw new Exception("PO already check in" + "\r\n" + "หมายเลขใบสั่งซื้อนี้ทำการ check in แล้ว");
            }

            for (int i = 0; i < bookingDtls.Count(); i++)
            {
                bookingDtls[i].Status = "DOCUMENT_CHECKIN";
                bookingDtls[i].DocumentCheckIn = DateTime.Now;
                bookingDtls[i].ModDate = DateTime.Now;
                bookingDtls[i].UserStamp = checkIn.UserStamp;

                unitOfWork.BookingDetailRepository.Update(bookingDtls[i]);

                PoCheckInOut poCheckInOut = new PoCheckInOut();
                poCheckInOut.PoNbr = checkIn.PoNbr;
                poCheckInOut.InternalTruckCheckinId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);
                poCheckInOut.UserStamp = checkIn.UserStamp;
                poCheckInOut.DateTimeStamp = DateTime.Now;
                poCheckInOut.BookingId = checkIn.BookingId;
                poCheckInOut.Status = "CHECKIN";

                unitOfWork.PoCheckInOutRepository.Add(poCheckInOut);

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CHECKIN PO";
                truckLog.Remark = checkIn.PoNbr;
                truckLog.UserStamp = checkIn.UserStamp;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);

                unitOfWork.BookingTruckLogRepository.Add(truckLog);

            }

            unitOfWork.Save();

            return await this.GetPoCheckInByInternalTruckCheckInId(checkIn.InternalTruckCheckInId.Value);

            //return null;
        }

        public async Task<List<PoCheckInOut>> PoNotInBooking(CheckInDto checkIn)
        {

            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByPoNo(checkIn.PoNbr);
            var bookingTruckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(checkIn.InternalTruckCheckInId.Value);

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

                    var bookingDtlId = unitOfWork.PoListRepository.GetBookingDetailKey();
                    unitOfWork.Save();

                    PoCheckInOut poCheckInOut = new PoCheckInOut();
                    poCheckInOut.PoNbr = checkIn.PoNbr;
                    poCheckInOut.InternalTruckCheckinId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);
                    poCheckInOut.UserStamp = checkIn.UserStamp;
                    poCheckInOut.DateTimeStamp = DateTime.Now;
                    poCheckInOut.BookingId = checkIn.BookingId;
                    poCheckInOut.Status = "CHECKIN";
                    unitOfWork.PoCheckInOutRepository.Add(poCheckInOut);

                    // save log
                    BookingTruckLog truckLog = new BookingTruckLog();
                    truckLog.Action = "CHECKIN PO NOT IN BOOKING";
                    truckLog.Remark = checkIn.PoNbr;
                    truckLog.UserStamp = checkIn.UserStamp;
                    truckLog.DateTimeStamp = DateTime.Now;
                    truckLog.InternalTruckCheckInId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);

                    unitOfWork.BookingTruckLogRepository.Add(truckLog);

                }
            }

            unitOfWork.Save();

            return await this.GetPoCheckInByInternalTruckCheckInId(checkIn.InternalTruckCheckInId.Value);

            //return null;
        }

        public async Task<List<PoCheckInOut>> DeletePoCheckIn(CheckInDto checkIn)
        {
            PoCheckInOut poCheckInOut = unitOfWork.PoCheckInOutRepository.GetPoCheckInOut(checkIn.BookingHeaderId.Value);

            unitOfWork.PoCheckInOutRepository.Delete(poCheckInOut);

            // save log
            BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.Action = "CANCEL CHECKIN PO";
            truckLog.Remark = checkIn.PoNbr;
            truckLog.UserStamp = checkIn.UserStamp;
            truckLog.DateTimeStamp = DateTime.Now;
            truckLog.InternalTruckCheckInId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);

            unitOfWork.BookingTruckLogRepository.Add(truckLog);

            unitOfWork.Save();

            return await this.GetPoCheckInByInternalTruckCheckInId(checkIn.InternalTruckCheckInId.Value);
        }

        public async Task<List<PoCheckInOut>> DeletePoUnload(CheckInDto checkIn)
        {
            PoCheckInOut poCheckInOut = unitOfWork.PoCheckInOutRepository.GetPoCheckInOut(checkIn.BookingHeaderId.Value);

            unitOfWork.PoCheckInOutRepository.Delete(poCheckInOut);

            // save log
            BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.Action = "CANCEL PO AFTER UNLOAD";
            truckLog.Remark = checkIn.PoNbr + ":" + checkIn.Remark;
            truckLog.UserStamp = checkIn.UserStamp;
            truckLog.DateTimeStamp = DateTime.Now;
            truckLog.InternalTruckCheckInId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);

            unitOfWork.BookingTruckLogRepository.Add(truckLog);

            unitOfWork.Save();

            return await this.GetPoCheckInByInternalTruckCheckInId(checkIn.InternalTruckCheckInId.Value);
        }

        #endregion

        #region +++ Document Check Out +++

        public async Task<List<PoCheckInOut>> GetPoCheckOutByInternalTruckCheckInId(int internalTruckCheckInId)
        {
            return unitOfWork.PoCheckInOutRepository.GetPoCheckOutsByInternalTruckCheckInId(internalTruckCheckInId).ToList();
        }

        public async Task<List<PoCheckInOut>> DocumentCheckOutPo(CheckInDto checkIn)
        {

            var poCheckIn = unitOfWork.PoCheckInOutRepository.GetPoCheckIn(checkIn.PoNbr);
            PoList po = new PoList();

            if (poCheckIn == null)
            {
                throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
            }

            PoCheckInOut poCheckInOut = new PoCheckInOut();
            poCheckInOut.PoNbr = checkIn.PoNbr;
            poCheckInOut.InternalTruckCheckinId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);
            poCheckInOut.UserStamp = checkIn.UserStamp;
            poCheckInOut.DateTimeStamp = DateTime.Now;
            poCheckInOut.BookingId = checkIn.BookingId;
            poCheckInOut.Status = "CHECKOUT";

            unitOfWork.PoCheckInOutRepository.Add(poCheckInOut);

            // save log
            BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.Action = "CHECKOUT PO";
            truckLog.Remark = checkIn.PoNbr;
            truckLog.UserStamp = checkIn.UserStamp;
            truckLog.DateTimeStamp = DateTime.Now;
            truckLog.InternalTruckCheckInId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);

            unitOfWork.BookingTruckLogRepository.Add(truckLog);


            unitOfWork.Save();

            return await this.GetPoCheckOutByInternalTruckCheckInId(checkIn.InternalTruckCheckInId.Value);

            //return null;
        }

        public async Task<List<PoCheckInOut>> DeletePoCheckOut(CheckInDto checkIn)
        {
            PoCheckInOut poCheckInOut = unitOfWork.PoCheckInOutRepository.GetPoCheckInOut(checkIn.BookingHeaderId.Value);

            unitOfWork.PoCheckInOutRepository.Delete(poCheckInOut);

            // save log
            BookingTruckLog truckLog = new BookingTruckLog();
            truckLog.Action = "CANCEL CHECKOUT PO";
            truckLog.Remark = checkIn.PoNbr;
            truckLog.UserStamp = checkIn.UserStamp;
            truckLog.DateTimeStamp = DateTime.Now;
            truckLog.InternalTruckCheckInId = Convert.ToDecimal(checkIn.InternalTruckCheckInId);

            unitOfWork.BookingTruckLogRepository.Add(truckLog);


            unitOfWork.Save();

            return await this.GetPoCheckOutByInternalTruckCheckInId(checkIn.InternalTruckCheckInId.Value);
        }

        #endregion

    }
}
