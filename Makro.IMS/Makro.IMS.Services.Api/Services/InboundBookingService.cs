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
    public class InboundBookingService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public InboundBookingService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public InboundBookingService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        #region +++ Manual booking +++

        public async Task<List<string>> CreateInboundBooking(InboundBookingDto manualBooking)
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

            bookingId = warehouse.OnlineBookingIdPrefix + DateTime.Now.Date.ToString("MMdd")
                    + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');

            Infra.Data.Models.BookingHeader bookingHdr = new Infra.Data.Models.BookingHeader();
            bookingHdr.InternalHeaderKey = unitOfWork.PoListRepository.GetBookingHeaderKey();
            bookingHdr.InternalKeyId = bookingKey.InternalKeyId;
            bookingHdr.InternalSupGroupId = manualBooking.SubGroupId;
            bookingHdr.InternalDoorId = 0;
            bookingHdr.BookingId = bookingId;
            
            //DateTime bookingStart = DateTime.Now;
            //DateTime bookingEnd = DateTime.Now;

            //DateTime checkInDateTime = DateTime.Now;

            //if(DateTime.Now.Minute > 30)
            //{
            //    bookingStart = bookingStart.AddMinutes(-1 * bookingStart.Minute);
            //    bookingStart = bookingStart.AddSeconds(-1 * bookingStart.Second);
            //    bookingStart = bookingStart.AddHours(1);
            //    bookingEnd = bookingStart.AddHours(1);
            //}
            //else
            //{
            //    bookingStart = bookingStart.AddMinutes(-1 * bookingStart.Minute);
            //    bookingStart = bookingStart.AddSeconds(-1 * bookingStart.Second);
            //    bookingEnd = bookingStart.AddHours(1);
            //}

            bookingHdr.BookingStart = manualBooking.StartTime.ToLocalTime();
            bookingHdr.BookingEnd = manualBooking.EndTime.ToLocalTime();
            bookingHdr.FirstBookginStart = manualBooking.StartTime.ToLocalTime();
            bookingHdr.FirstBookingEnd = manualBooking.EndTime.ToLocalTime();
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
            bookingHdr.Status = "INTRANSIT";
            bookingHdr.UserStamp = manualBooking.UserName;
            bookingHdr.CreateDate = DateTime.Now;
            bookingHdr.Remark = "";
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
            truckCheckIn.Status = "INTRANSIT";
            truckCheckIn.DateTimeStamp = DateTime.Now;
            truckCheckIn.UserStamp = manualBooking.UserName;
            truckCheckIn.LineId = manualBooking.LineNo;
            //truckCheckIn.ArrivedTime = checkInDateTime;

            unitOfWork.BookingTruckCheckInRepository.AddInboundBooking(truckCheckIn);

            unitOfWork.Save();

            List<string> strResult = new List<string>();
            strResult.Add(warehouse.WarehouseCode + " : " + bookingId);

            return strResult;
        }

        #endregion


    }
}
