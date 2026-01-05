using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Controllers;
using Makro.IMS.Services.Api.Dto;
using System.Linq;
using System.Xml.Linq;

namespace Makro.IMS.Services.Api.Services
{
    public class PreCheckInExcelService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private WarehouseCapacityService warehouseCapacityService;        

        public PreCheckInExcelService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            warehouseCapacityService = new WarehouseCapacityService();
        }

        public async Task<PreCheckInExcelDto> Import(PreCheckInExcelDto preCheckInExcelDto,string userName)
        {
            List<TruckMaster> truckTypes = new List<TruckMaster>();
            truckTypes = unitOfWork.TruckMasterRepository.GetTrucks().ToList();
            
            List<BookingTruckCheckIn> bookingTruckCheckIns = new List<BookingTruckCheckIn>();
            BookingTruckCheckIn bookingTruckCheckIn = new BookingTruckCheckIn();

            List<BookingTruckCheckInDetail> bookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();
            BookingTruckCheckInDetail bookingTruckCheckInDetail = new BookingTruckCheckInDetail();

            BookingHeader bookingHeader = new BookingHeader();

            var bookingIdLists = new List<string>();
            var prevTruckLicense = "";
            var prevBookingId = "";

            bookingIdLists = preCheckInExcelDto.PreCheckInExcelDetailDtos.Select(x=>x.BookingId).Distinct().ToList();

            foreach (var bookingId in bookingIdLists)
            {
                // check booking id
                var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingId);
                if(bookingHdr != null)
                {
                    // check booking status
                    if(bookingHdr.Status != "APPROVED")
                    {
                        foreach (var item in preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x => x.BookingId == bookingId).ToList())
                        {
                            item.ImportResult += "Booking id is invalid status (" + bookingHdr.Status + ")." + "\r\n";
                        }
                    }
                    // check truck type and truck license plate
                    foreach (var item in preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x => x.BookingId == bookingId).ToList())
                    {                        
                        if (truckTypes.FirstOrDefault(x=>x.TruckCode == item.TruckType) == null)
                        {
                            item.ImportResult += "Truck type is invalid." + "\r\n";
                        }
                    }
                    // check truck license plate duplicate
                    foreach (var item in preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x => x.BookingId == bookingId).ToList())
                    {
                        var licensePlate = item.TruckLicense + "|" + (item.TruckLicense2 == null ? "" :  item.TruckLicense2);
                        var checkDup = unitOfWork.PoListRepository.CheckDuplicateTruck(bookingHdr.BookingStart.Value, bookingHdr.BookingId, licensePlate);
                        

                        if (checkDup > 0)
                        {
                            item.ImportResult += "License plate " + licensePlate + " is duplicate in same booking time." + "\r\n";
                        }
                    }

                }
                else
                {
                    foreach (var item in preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x => x.BookingId == bookingId).ToList())
                    {
                        item.ImportResult += "Booking id not found." + "\r\n";
                    }
                }
            }

            var bookingError = preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x=>x.ImportResult.Length > 0).Select(x=>x.BookingId).Distinct().ToList();

            var bookingPass = bookingIdLists.Except(bookingError).ToList();

            var isFirst = true;
            
            
            foreach (var truckAssign in preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x=>bookingPass.Contains(x.BookingId)).OrderBy(x=>x.BookingId).ThenByDescending(x=>x.TruckSequence))
            {
                if (prevBookingId != truckAssign.BookingId)
                {
                    isFirst = true;
                    bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(truckAssign.BookingId);
                    bookingHeader.Status = "INTRANSIT";
                    bookingHeader.ModDate = DateTime.Now;
                    bookingHeader.UserStamp = userName;

                    unitOfWork.BookingHeaderRepository.Update(bookingHeader);
                }
                else
                {
                    isFirst = false;
                }

                var checkInId = unitOfWork.PoListRepository.GetBookingTruckCheckInKey();
                unitOfWork.Save();

                // create booking truck check in
                bookingTruckCheckIn = new BookingTruckCheckIn();
                bookingTruckCheckIn.InternalTruckCheckInId = checkInId;
                bookingTruckCheckIn.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingTruckCheckIn.InternalTruckId = truckTypes.FirstOrDefault(x => x.TruckCode == truckAssign.TruckType).InternalTruckId;
                bookingTruckCheckIn.DriverName = truckAssign.DriverName;
                bookingTruckCheckIn.LicensePlate = truckAssign.TruckLicense;
                bookingTruckCheckIn.LicensePlate2 = truckAssign.TruckLicense2;
                bookingTruckCheckIn.LineId = truckAssign.LineNo;
                bookingTruckCheckIn.TelNo = truckAssign.TelNo;
                bookingTruckCheckIn.UserStamp = userName;
                bookingTruckCheckIn.DateTimeStamp = DateTime.Now;
                bookingTruckCheckIn.Status = "INTRANSIT";

                unitOfWork.BookingTruckCheckInRepository.Add(bookingTruckCheckIn);

                unitOfWork.Save();

                if (isFirst)
                {
                    // add po to first truck
                    var pos = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeader.InternalHeaderKey).ToList();

                    foreach (var po in pos)
                    {
                        var checkInDtlId = unitOfWork.PoListRepository.GetBookingTruckCheckInDtlKey();
                        unitOfWork.Save();

                        bookingTruckCheckInDetail = new BookingTruckCheckInDetail();
                        bookingTruckCheckInDetail.InternalTruckCheckInId = bookingTruckCheckIn.InternalTruckCheckInId;
                        bookingTruckCheckInDetail.InternalTruckDetailId = checkInDtlId;
                        bookingTruckCheckInDetail.PoNbr = po.PoNbr;
                        bookingTruckCheckInDetail.InternalDetailKey = po.InternalDetailKey;                        

                        po.PreCheckIn = DateTime.Now;
                        po.Status = "INTRANSIT";
                        po.UserStamp = userName;
                        po.ModDate = DateTime.Now;

                        unitOfWork.BookingTruckCheckInDetailRepository.Add(bookingTruckCheckInDetail);
                        unitOfWork.BookingDetailRepository.Update(po);

                        unitOfWork.Save();
                    }

                }

                prevBookingId = truckAssign.BookingId;                
            }

            // stamp import result;
            preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x => !bookingError.Contains(x.BookingId)).ToList().ForEach(delegate (PreCheckInExcelDetailDto preCheckIn)
            {
                preCheckIn.ImportResult = "Completed";
            });

            foreach (var booking in bookingError)
            {
                var resultMsg = "";
                preCheckInExcelDto.PreCheckInExcelDetailDtos.Where(x => x.BookingId == booking).ToList().ForEach(delegate (PreCheckInExcelDetailDto preCheckIn)
                {
                    if(preCheckIn.ImportResult.Length == 0)
                    {
                        preCheckIn.ImportResult = "Can't import because some row in booking id error";
                    }
                });
            }

            return preCheckInExcelDto;
        }
    }
}
