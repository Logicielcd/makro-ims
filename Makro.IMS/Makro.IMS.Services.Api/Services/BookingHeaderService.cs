using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;
using System.Net;
using System.Reflection.Emit;

namespace Makro.IMS.Services.Api.Services
{
    public class BookingHeaderService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public BookingHeaderService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<BookingHeader>> GetBookingsAsync()
        {
            return await Task.Run<List<BookingHeader>>(() => unitOfWork.BookingHeaderRepository.GetBookingHeaders().ToList());
        }

        public async Task<BookingHeader> GetBookingsByIdAsync(int keyId)
        {
            return await Task.Run<BookingHeader>(() => unitOfWork.BookingHeaderRepository.GetBookingHeaderById(keyId)!);
        }

        public async Task<List<BookingHeader>> GetBookingHeaderBySupCodeAndBookingDate(string supCode, DateTime bookingDate)
        {
            var result = await Task.Run<IEnumerable<BookingHeader>>(() => unitOfWork.BookingHeaderRepository.GetBookingHeaderBySupplierAndBookingDate(supCode, bookingDate));            
            return result.ToList();
        }

        public async Task<PagedResult<BookingHeader>> GetBookingsPaged(SieveModel sieveModel,string userName)
        {
            var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);
            
            // get user warehouse
            var userWhses = unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(user.UserId).ToList();
            var whseList = new List<string>();
            
            if(user.WarehouseCode != null)
            {
                whseList = userWhses.Select(x => x.WarehouseCode).ToList();
                whseList.Add(user.WarehouseCode);
            }

            // get user supplier group
            var userSupGroups = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(user.UserId).ToList();
            var supGroupList = new List<int>();
            
            if(user.InternalSupGroupId != null)
            {
                supGroupList = userSupGroups.Select(x => x.InternalSupGroup).ToList();
                supGroupList.Add(Convert.ToInt32(user.InternalSupGroupId.Value));
            }
            
            if(whseList.Count > 0 && supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetBookingHeadersPaged(whseList, supGroupList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            if(whseList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetBookingHeadersPaged(whseList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            if(supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetBookingHeadersPaged(supGroupList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            var queryableResult = unitOfWork.BookingHeaderRepository.GetBookingHeadersPaged();
            return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryableResult, sieveModel);

            //if (user.UserType != "SUP" && user.UserType != "SUPTRAN")
            //{
            //    var queryable = unitOfWork.BookingHeaderRepository.GetBookingHeadersPaged();
            //    return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            //}
            //else
            //{
            //    //int supGroupId = unitOfWork.SupplierRepository.GetSupplierBySupCode(user.InternalSupGroupId.Value.ToString()).InternalGroupId.Value;
            //    var queryable = unitOfWork.BookingHeaderRepository.GetBookingHeadersPaged(Convert.ToInt32(user.InternalSupGroupId.Value));
            //    return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            //}
        }

        public async Task<PagedResult<BookingHeader>> GetPreCheckInPaged(SieveModel sieveModel, string userName)
        {
            var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);

            // get user warehouse
            var userWhses = unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(user.UserId).ToList();
            var whseList = new List<string>();

            if (user.WarehouseCode != null)
            {
                whseList = userWhses.Select(x => x.WarehouseCode).ToList();
                whseList.Add(user.WarehouseCode);
            }

            // get user supplier group
            var userSupGroups = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(user.UserId).ToList();
            var supGroupList = new List<int>();

            if (user.InternalSupGroupId != null)
            {
                supGroupList = userSupGroups.Select(x => x.InternalSupGroup).ToList();
                supGroupList.Add(Convert.ToInt32(user.InternalSupGroupId.Value));
            }

            if (whseList.Count > 0 && supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(whseList, supGroupList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            if (whseList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(whseList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            if (supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(supGroupList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            var queryableResult = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged();
            return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryableResult, sieveModel);

            //if (user.UserType != "SUP" && user.UserType != "SUPTRAN")
            //{
            //    var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged();
            //    return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            //}
            //else
            //{
            //    //int supGroupId = unitOfWork.SupplierRepository.GetSupplierBySupCode(user.InternalSupGroupId.Value.ToString()).InternalGroupId.Value;
            //    var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(Convert.ToInt32(user.InternalSupGroupId.Value));
            //    return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            //}
        }

        public async Task<PagedResult<BookingHeaderDto>> GetPreCheckInPagedExport(SieveModel sieveModel, string userName)
        {
            var result = new PagedResult<BookingHeader>();

            var returnResult = new PagedResult<BookingHeaderDto>();

            //var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);

            //if (user.UserType != "SUP" && user.UserType != "SUPTRAN")
            //{
            //    var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged();
            //    result = await _sieveProcessor.GetPagedExportAsync<BookingHeader>(queryable, sieveModel);
            //}
            //else
            //{
            //    //int supGroupId = unitOfWork.SupplierRepository.GetSupplierBySupCode(user.InternalSupGroupId.Value.ToString()).InternalGroupId.Value;
            //    var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(Convert.ToInt32(user.InternalSupGroupId.Value));
            //    result = await _sieveProcessor.GetPagedExportAsync<BookingHeader>(queryable, sieveModel);
            //}

            var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);

            // get user warehouse
            var userWhses = unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(user.UserId).ToList();
            var whseList = new List<string>();

            if (user.WarehouseCode != null)
            {
                whseList = userWhses.Select(x => x.WarehouseCode).ToList();
                whseList.Add(user.WarehouseCode);
            }

            // get user supplier group
            var userSupGroups = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(user.UserId).ToList();
            var supGroupList = new List<int>();

            if (user.InternalSupGroupId != null)
            {
                supGroupList = userSupGroups.Select(x => x.InternalSupGroup).ToList();
                supGroupList.Add(Convert.ToInt32(user.InternalSupGroupId.Value));
            }

            if (whseList.Count > 0 && supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(whseList, supGroupList);
                result = await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }
            else if (whseList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(whseList);
                result = await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }
            else if (supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged(supGroupList);
                result = await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }
            else
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInPaged();
                result = await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            // convert to bookingDto
            List<BookingHeaderDto> bookingHdrDtos = new List<BookingHeaderDto>();
            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            var truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

            foreach (var item in result.Results.OrderBy(x => x.WarehouseCode).ThenBy(x => x.BookingId))
            {
                bookingHeaderDto = new BookingHeaderDto();
                bookingHeaderDto.BookingId = item.BookingId;
                bookingHeaderDto.WarehouseCode = item.WarehouseCode;
                bookingHeaderDto.BookingDetails = new List<BookingDetail>();
                bookingHeaderDto.InternalHeaderKey = item.InternalHeaderKey;
                bookingHeaderDto.BookingStart = item.BookingStart.Value;
                bookingHeaderDto.BookingEnd = item.BookingEnd.Value;
                bookingHeaderDto.BookingTrucks = new List<BookingTruck>();

                var dtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(item.InternalHeaderKey).ToList();

                foreach (var dtl in dtls.OrderBy(x => x.PoNbr))
                {
                    BookingDetail bd = new BookingDetail();
                    bd.PoNbr = dtl.PoNbr;
                    bd.InternalDetailKey = dtl.InternalDetailKey;
                    bd.InternalHeaderKey = dtl.InternalHeaderKey;
                    bd.CubeCon = dtl.CubeCon;
                    bd.Con = dtl.Con;
                    bd.Non = dtl.Non;
                    bd.CubeNon = dtl.CubeNon;
                    bd.FullPl = dtl.FullPl;
                    bd.CubeFull = dtl.CubeFull;
                    bd.TotalQty = dtl.TotalQty;

                    bookingHeaderDto.BookingDetails.Add(bd);
                }

                var trucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(item.InternalHeaderKey).ToList();

                foreach (var truck in trucks.OrderBy(x => x.InternalTruckId))
                {
                    BookingTruck bt = new BookingTruck();
                    bt.InternalTruckId = truck.InternalTruckId;
                    bt.InternalHeaderKey = truck.InternalHeaderKey;
                    bt.TotalTruck = truck.TotalTruck;
                    bt.Remark = truckMasters.FirstOrDefault(x => x.InternalTruckId == truck.InternalTruckId).Sequence;
                    bookingHeaderDto.BookingTrucks.Add(bt);
                }

                bookingHdrDtos.Add(bookingHeaderDto);
                returnResult.Results.Add(bookingHeaderDto);
            }


            return returnResult;
        }

        public async Task<PagedResult<BookingHeader>> GetPreCheckInCompletedPaged(SieveModel sieveModel, string userName)
        {
            var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);

            // get user warehouse
            var userWhses = unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(user.UserId).ToList();
            var whseList = new List<string>();

            if (user.WarehouseCode != null)
            {
                whseList = userWhses.Select(x => x.WarehouseCode).ToList();
                whseList.Add(user.WarehouseCode);
            }

            // get user supplier group
            var userSupGroups = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(user.UserId).ToList();
            var supGroupList = new List<int>();

            if (user.InternalSupGroupId != null)
            {
                supGroupList = userSupGroups.Select(x => x.InternalSupGroup).ToList();
                supGroupList.Add(Convert.ToInt32(user.InternalSupGroupId.Value));
            }

            if (whseList.Count > 0 && supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInCompletedPaged(whseList, supGroupList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            if (whseList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInCompletedPaged(whseList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            if (supGroupList.Count > 0)
            {
                var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInCompletedPaged(supGroupList);
                return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            }

            var queryableResult = unitOfWork.BookingHeaderRepository.GetPreCheckInCompletedPaged();
            return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryableResult, sieveModel);

            //var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);

            //if (user.UserType != "SUP" && user.UserType != "SUPTRAN")
            //{
            //    var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInCompletedPaged();
            //    return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            //}
            //else
            //{
            //    var queryable = unitOfWork.BookingHeaderRepository.GetPreCheckInCompletedPaged(Convert.ToInt32(user.InternalSupGroupId.Value));
            //    return await _sieveProcessor.GetPagedAsync<BookingHeader>(queryable, sieveModel);
            //}
        }

        public async Task<List<BookingHeader>> GetDashboard(string warehouseCode)
        {
            return await Task.Run<List<BookingHeader>>(
                () => unitOfWork.BookingHeaderRepository.GetDashboard(warehouseCode).ToList()
                );
        }

        public async Task<BookingHeaderDto> GetBookingByPo(string poNo, string supCode)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            Supplier sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(supCode);

            bookingDetail = unitOfWork.BookingDetailRepository.GetBookingDetailByPoNo(poNo);

            if (bookingDetail != null)
            {
                bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingDetail.InternalHeaderKey.Value);
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }

            List<string> statuses = new List<string>();

            statuses.Add("CHECKIN_COMPLETED");
            statuses.Add("CHECKOUT_PROCESS");
            statuses.Add("COMPLETED");

            if (bookingHeader.Status.ToUpper() == "NEW" || bookingHeader.Status.ToUpper() == "OVERCAP")
            {
                //throw new Exception("This booking on the approve process");
                throw new Exception("Booking Id on the approve process" + "\r\n" + "หมายเลขนัดหมายกำลังอยู่ในขั้นตอนการพิจารณา");
            }

            if (statuses.Contains(bookingHeader.Status))
            {
                throw new Exception("Booking Id already check-in" + "\r\n" + "หมายเลขนัดหมายถูก check-in แล้ว");
            }


            if (bookingHeader.InternalSupGroupId == sup.InternalGroupId)
            {
                bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingHeaderDto.SupCode = bookingHeader.SupCode;
                bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
                bookingHeaderDto.BookingId = bookingHeader.BookingId;
                bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
                bookingHeaderDto.DockDoor = unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
                bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
                bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;
                bookingHeaderDto.Status = bookingHeader.Status;

                truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

                // get booking detail
                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                // get booking truck
                bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                bookingTruckCheckIns.ForEach((truckCheckIn) =>
                {
                    BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                    checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckCheckIn.InternalTruckCheckInId);
                    checkInDto.InternalTruckId = Convert.ToInt32(truckCheckIn.InternalTruckId);
                    checkInDto.DriverName = truckCheckIn.DriverName.Trim();
                    checkInDto.LicensePlate = truckCheckIn.LicensePlate.Trim();
                    checkInDto.TelNo = truckCheckIn.TelNo;
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();

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
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }
        }

        public async Task<BookingHeaderDto> GetBookingByBooking(string bookingId, string supCode)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            Supplier sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(supCode);

            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingId);

            if(bookingHeader == null)
            {                
                throw new Exception("Incorrect Booking Id" + "\r\n" + "หมายเลขนัดหมายผิด");
            }

            List<string> statuses = new List<string>();

            statuses.Add("CHECKIN_COMPLETED");
            statuses.Add("CHECKOUT_PROCESS");
            statuses.Add("COMPLETED");


            if (bookingHeader.Status.ToUpper() == "NEW" || bookingHeader.Status.ToUpper() == "OVERCAP")
            {
                //throw new Exception("This booking on the approve process");
                throw new Exception("Booking Id on the approve process" + "\r\n" + "หมายเลขนัดหมายกำลังอยู่ในขั้นตอนการพิจารณา");
            }

            if (statuses.Contains(bookingHeader.Status))
            {
                throw new Exception("Booking Id already check-in" + "\r\n" + "หมายเลขนัดหมายถูก check-in แล้ว");
            }


            if (bookingHeader.InternalSupGroupId == sup.InternalGroupId)
            {
                bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingHeaderDto.SupCode = bookingHeader.SupCode;
                bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
                bookingHeaderDto.BookingId = bookingHeader.BookingId;
                bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
                bookingHeaderDto.DockDoor = bookingHeader.InternalDoorId.Value == 0 ? "-" : unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
                bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
                bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;
                bookingHeaderDto.Status = bookingHeader.Status;
                bookingHeaderDto.RemarkDelay = bookingHeader.RemarkDelay;

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
                    checkInDto.DriverName = truckCheckIn.DriverName.Trim();
                    checkInDto.LicensePlate = truckCheckIn.LicensePlate.Trim();
                    checkInDto.LicensePlate2 = truckCheckIn.LicensePlate2;
                    checkInDto.TelNo = truckCheckIn.TelNo;
                    checkInDto.LineId = truckCheckIn.LineId;
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();

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
                    strTotalTruck += strTotalTruck == "" ? "" : "|"; 
                    strTotalTruck += truckMasters.Find(x => x.InternalTruckId == item.truckId).TruckCode + "-[" + item.totalTruck.ToString() + "]";
                }

                bookingHeaderDto.BookingDetails = bookingDetails;
                bookingHeaderDto.BookingTrucks = bookingTrucks;
                bookingHeaderDto.TotalTruck = strTotalTruck;
                bookingHeaderDto.BookingCheckIns = bookingCheckIns;

                return bookingHeaderDto;
            }
            else
            {
                throw new Exception("Incorrect Booking Id" + "\r\n" + "หมายเลขนัดหมายผิด");
            }
        }

        public async Task<BookingHeaderDto> GetBookingByBookingHeaderKey(int bookingHeaderKey, string supCode)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();


            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderKey);

            if (bookingHeader == null)
            {
                throw new Exception("Incorrect Booking Id" + "\r\n" + "หมายเลขนัดหมายผิด");
            }

            List<string> statuses = new List<string>();

            statuses.Add("CHECKIN_COMPLETED");
            statuses.Add("CHECKOUT_PROCESS");
            statuses.Add("COMPLETED");


            if (bookingHeader.Status.ToUpper() == "NEW" || bookingHeader.Status.ToUpper() == "OVERCAP")
            {
                //throw new Exception("This booking on the approve process");
                throw new Exception("Booking Id on the approve process" + "\r\n" + "หมายเลขนัดหมายกำลังอยู่ในขั้นตอนการพิจารณา");
            }

            if (statuses.Contains(bookingHeader.Status))
            {
                throw new Exception("Booking Id already check-in" + "\r\n" + "หมายเลขนัดหมายถูก check-in แล้ว");
            }


            Supplier sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(bookingHeader.SupCode);

            if (bookingHeader.InternalSupGroupId == sup.InternalGroupId)
            {
                bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingHeaderDto.SupCode = bookingHeader.SupCode;
                bookingHeaderDto.SupName = sup.SupName;
                
                bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
                bookingHeaderDto.BookingId = bookingHeader.BookingId;
                bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
                bookingHeaderDto.DockDoor = unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
                bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
                bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;
                bookingHeaderDto.Status = bookingHeader.Status;
                bookingHeaderDto.RemarkDelay = bookingHeader.RemarkDelay;

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
                    checkInDto.DriverName = truckCheckIn.DriverName.Trim();
                    checkInDto.LicensePlate = truckCheckIn.LicensePlate.Trim();
                    checkInDto.LicensePlate2 = truckCheckIn.LicensePlate2;
                    checkInDto.TelNo = truckCheckIn.TelNo;
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();

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
            else
            {
                throw new Exception("Incorrect Booking Id" + "\r\n" + "หมายเลขนัดหมายผิด");
            }
        }

        public async Task<List<BookingCheckInDto>> GetBookingCheckIn(string supCode,string warehouseCode)
        {
            var sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(supCode);

            var result = unitOfWork.PoListRepository.GetBookingCheckIn(sup.InternalGroupId.Value);
            return result;
        }

        public async Task<List<BookingTruckCheckInDto>> SaveCheckIn(BookingTruckCheckInDto checkIn)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            BookingTruckCheckIn bookingCheckIn = new BookingTruckCheckIn();
            BookingTruckCheckInDetail bookingCheckInDetail = new BookingTruckCheckInDetail();
            List<BookingTruckCheckInDto> result = new List<BookingTruckCheckInDto>();
            int headerKey = 0;
            headerKey = Convert.ToInt32(checkIn.InternalHeaderKey);
            List<string> statuses = new List<string>();

            statuses.Add("CHECKIN_COMPLETED");
            statuses.Add("CHECKOUT_PROCESS");
            statuses.Add("COMPLETED");


            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(Convert.ToInt32(checkIn.InternalHeaderKey));

            if (statuses.Contains(bookingHeader.Status))
            {
                //throw new Exception("This booking already check-in");
                throw new Exception("Booking Id already check-in" + "\r\n" + "หมายเลขนัดหมายถูก check-in แล้ว");
            }

            bookingDetail = unitOfWork.BookingDetailRepository.GetBookingDetailByPoNoAndId(checkIn.PoNo, headerKey);

            if (bookingDetail == null)
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }
            else
            {

                if (bookingDetail.Status == "CHECKIN")
                {
                    var bookingCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(headerKey).ToList();
                    if (bookingCheckIns.Count(x => x.LicensePlate == checkIn.LicensePlate.Trim()) > 0)
                    {
                        throw new Exception("PO already checkin" + "\r\n" + "หมายเลขใบสั่งซื้อถูก check-in แล้ว");
                    }
                    else
                    {
                        // check booking check in                         
                        bookingCheckIn = new BookingTruckCheckIn();
                        bookingCheckIn.InternalTruckId = checkIn.InternalTruckId;
                        bookingCheckIn.InternalHeaderKey = checkIn.InternalHeaderKey;
                        //bookingCheckIn.InternalDetailKey = bookingDetail.InternalDetailKey;
                        //bookingCheckIn.PoNbr = checkIn.PoNbr;
                        bookingCheckIn.LicensePlate = checkIn.LicensePlate.Trim();
                        bookingCheckIn.LicensePlate2 = checkIn.LicensePlate2;
                        bookingCheckIn.DriverName = checkIn.DriverName.Trim();
                        bookingCheckIn.UserStamp = checkIn.UserStamp;
                        bookingCheckIn.TelNo = checkIn.TelNo;
                        bookingCheckIn.CheckInTime = DateTime.Now;

                        unitOfWork.BookingTruckCheckInRepository.Add(bookingCheckIn);
                        unitOfWork.Save();

                        // get booking truck check in
                        bookingCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(checkIn.InternalHeaderKey).FirstOrDefault(x => x.LicensePlate == checkIn.LicensePlate && x.DriverName == checkIn.DriverName);

                        bookingCheckInDetail = new BookingTruckCheckInDetail();
                        bookingCheckInDetail.InternalDetailKey = bookingDetail.InternalDetailKey;
                        bookingCheckInDetail.PoNbr = checkIn.PoNo;
                        bookingCheckInDetail.CheckInTime = DateTime.Now;
                        bookingCheckInDetail.InternalTruckCheckInId = bookingCheckIn.InternalTruckCheckInId;
                        unitOfWork.BookingTruckCheckInDetailRepository.Add(bookingCheckInDetail);
                        unitOfWork.Save();
                    }
                }
                else
                {
                    var bookingCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(headerKey).ToList();
                    decimal internalCheckInId = 0;
                    if (bookingCheckIns.Count(x => x.LicensePlate == checkIn.LicensePlate.Trim()) > 0)
                    {
                        internalCheckInId = bookingCheckIns.FirstOrDefault(x => x.LicensePlate == checkIn.LicensePlate.Trim()).InternalTruckCheckInId;
                    }
                    else
                    { 
                        
                        bookingCheckIn = new BookingTruckCheckIn();
                        bookingCheckIn.InternalTruckId = checkIn.InternalTruckId;
                        bookingCheckIn.InternalHeaderKey = checkIn.InternalHeaderKey;                        
                        //bookingCheckIn.InternalDetailKey = bookingDetail.InternalDetailKey;
                        //bookingCheckIn.PoNbr = checkIn.PoNbr;
                        bookingCheckIn.DriverName = checkIn.DriverName.Trim();
                        bookingCheckIn.LicensePlate = checkIn.LicensePlate.Trim();
                        bookingCheckIn.LicensePlate2 = checkIn.LicensePlate2;
                        bookingCheckIn.TelNo = checkIn.TelNo;
                        bookingCheckIn.UserStamp = checkIn.UserStamp;
                        bookingCheckIn.CheckInTime = DateTime.Now;
                        
                        unitOfWork.BookingTruckCheckInRepository.Add(bookingCheckIn);
                        unitOfWork.Save();
                    }


                    bookingDetail.Status = "CHECKIN";
                    bookingDetail.CheckIn = DateTime.Now;
                    unitOfWork.BookingDetailRepository.Update(bookingDetail);
                    unitOfWork.Save();

                    // get booking truck check in
                    bookingCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(checkIn.InternalHeaderKey).FirstOrDefault(x=>x.LicensePlate == checkIn.LicensePlate && x.DriverName == checkIn.DriverName);

                    bookingCheckInDetail = new BookingTruckCheckInDetail();
                    bookingCheckInDetail.InternalDetailKey = bookingDetail.InternalDetailKey;
                    bookingCheckInDetail.PoNbr = checkIn.PoNo;
                    bookingCheckInDetail.CheckInTime = DateTime.Now;
                    bookingCheckInDetail.InternalTruckCheckInId = bookingCheckIn.InternalTruckCheckInId;
                    unitOfWork.BookingTruckCheckInDetailRepository.Add(bookingCheckInDetail);
                    unitOfWork.Save();
                }

                List<BookingDetail> bookingDetails = new List<BookingDetail>();
                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(Convert.ToInt32(checkIn.InternalHeaderKey)).ToList();


                int totalCheckIn = bookingDetails.Count(x => x.Status.ToUpper() == "CHECKIN");

                if (totalCheckIn == bookingDetails.Count())
                {
                    bookingHeader.Status = "CHECKIN_COMPLETED";
                }
                else
                {
                    bookingHeader.Status = "CHECKIN_PROCESS";
                }

                unitOfWork.BookingHeaderRepository.Update(bookingHeader);
                unitOfWork.Save();

                var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(Convert.ToInt32(checkIn.InternalHeaderKey)).ToList();                

                bookingTruckCheckIns.ForEach((truckIn) =>
                {
                    BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                    checkInDto.InternalTruckId = Convert.ToInt32(truckIn.InternalTruckId);
                    checkInDto.DriverName = truckIn.DriverName;
                    checkInDto.TelNo = truckIn.TelNo;
                    checkInDto.LicensePlate = truckIn.LicensePlate.Trim();
                    checkInDto.LicensePlate2 = truckIn.LicensePlate2;
                    checkInDto.InternalHeaderKey = Convert.ToInt32(truckIn.InternalHeaderKey);
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail?>();
                    checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckIn.InternalTruckCheckInId);

                    var bookingTruckDetails = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(truckIn.InternalTruckCheckInId)).ToList();

                    bookingTruckDetails.ForEach((truckDetail) =>
                    {
                        checkInDto.BookingTruckCheckInDetails.Add(truckDetail);
                    });

                    result.Add(checkInDto);
                });
                
                

                return result;
            }
            //return null;
        }

        public async Task<BookingHeaderDto> GetPoCheckOut(string poNo, string supCode)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            bookingDetail = unitOfWork.BookingDetailRepository.GetBookingDetailByPoNo(poNo);

            if (bookingDetail != null)
            {
                bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingDetail.InternalHeaderKey.Value);
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }

            List<string> statuses = new List<string>();

            statuses.Add("NEW");
            statuses.Add("CHECKIN_PROCESS");
            
            if (statuses.Contains(bookingHeader.Status))
            {
                //throw new Exception("This booking cannot check-out");
                throw new Exception("Booking Id cannot check-out" + "\r\n" + "หมายเลขนัดหมายไม่สามารถ check-out ได้");
            }

            if(bookingHeader.Status == "COMPLETED")
            {
                throw new Exception("Booking Id already check-out" + "\r\n" + "หมายเลขนัดหมายถูก check-out แล้ว");
            }

            if (bookingHeader.SupCode == supCode)
            {
                bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingHeaderDto.SupCode = bookingHeader.SupCode;
                bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
                bookingHeaderDto.BookingId = bookingHeader.BookingId;
                bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
                bookingHeaderDto.DockDoor = unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
                bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
                bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;

                truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

                // get booking detail
                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                // get booking truck
                bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                bookingTruckCheckIns.ForEach((truckCheckIn) =>
                {
                    BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                    checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckCheckIn.InternalTruckCheckInId);
                    checkInDto.InternalTruckId = Convert.ToInt32(truckCheckIn.InternalTruckId);
                    checkInDto.DriverName = truckCheckIn.DriverName;
                    checkInDto.LicensePlate = truckCheckIn.LicensePlate;
                    checkInDto.LicensePlate2 = truckCheckIn.LicensePlate2;
                    checkInDto.TelNo = truckCheckIn.TelNo;
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();

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
                bookingHeaderDto.BookingCheckIns = new List<BookingTruckCheckInDto>();
                bookingHeaderDto.BookingCheckIns = bookingCheckIns;

                return bookingHeaderDto;
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }
        }

        public async Task<BookingHeaderDto> SaveOldCheckOut(BookingCheckOutDto checkOut)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            Supplier sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(checkOut.SupCode);

            bookingDetail = unitOfWork.BookingDetailRepository.GetBookingDetailByPoNo(checkOut.PoNo);

            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingDetail.InternalHeaderKey.Value);

            //if (bookingHeader.Status == "COMPLETED")
            //{
            //    throw new Exception("This booking already check-out");
            //}


            if (bookingDetail != null)
            {
                // check booking


                // check out PO
                bookingDetail.Status = "CHECKOUT";
                bookingDetail.CheckOut = DateTime.Now;

                unitOfWork.BookingDetailRepository.Update(bookingDetail);

                unitOfWork.Save();

                bookingDetails = new List<BookingDetail>();

                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                int totalCheckIn = bookingDetails.Count(x => x.Status.ToUpper() == "CHECKOUT");


                if (totalCheckIn == bookingDetails.Count())
                {
                    bookingHeader.Status = "COMPLETED";
                    bookingHeader.ModDate = DateTime.Now;
                }
                else
                {
                    bookingHeader.Status = "CHECKOUT_PROCESS";
                    bookingHeader.ModDate = DateTime.Now;
                }

                unitOfWork.BookingHeaderRepository.Update(bookingHeader);
                unitOfWork.Save();

                //bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingDetail.InternalHeaderKey.Value);
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }

            List<string> statuses = new List<string>();

            statuses.Add("NEW");
            statuses.Add("CHECKIN_PROCESS");

            if (statuses.Contains(bookingHeader.Status))
            {
                throw new Exception("This booking cannot check-out" + "\r\n" + "หมายเลขนัดหมายนี้ไม่สามารถทำการ check-out ได้");
            }

            if (bookingHeader.InternalSupGroupId == sup.InternalGroupId)
            {
                bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingHeaderDto.SupCode = bookingHeader.SupCode;
                bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
                bookingHeaderDto.BookingId = bookingHeader.BookingId;
                bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
                bookingHeaderDto.DockDoor = unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName;
                bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
                bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;

                truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

                // get booking detail
                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                // get booking truck
                bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                bookingTruckCheckIns.ForEach((truckCheckIn) =>
                {
                    BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                    checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckCheckIn.InternalTruckCheckInId);
                    checkInDto.InternalTruckId = Convert.ToInt32(truckCheckIn.InternalTruckId);
                    checkInDto.DriverName = truckCheckIn.DriverName;
                    checkInDto.LicensePlate = truckCheckIn.LicensePlate;
                    checkInDto.LicensePlate2 = truckCheckIn.LicensePlate2;
                    checkInDto.TelNo = truckCheckIn.TelNo;
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();

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
                bookingHeaderDto.BookingCheckIns = new List<BookingTruckCheckInDto>();
                bookingHeaderDto.BookingCheckIns = bookingCheckIns;

                return bookingHeaderDto;
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }
        }

        public async Task<BookingHeaderDto> SaveCheckOut(BookingCheckOutDto checkOut)
        {
            BookingDetail bookingDetail = new BookingDetail();
            BookingHeader bookingHeader = new BookingHeader();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            Supplier sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(checkOut.SupCode);

            bookingDetail = unitOfWork.BookingDetailRepository.GetBookingDetailByPoNo(checkOut.PoNo);

            if(bookingDetail == null)
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }

            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingDetail.InternalHeaderKey.Value);

            List<string> status = new List<string>();
            status.Add("CHECKOUT_PROCESS");
            status.Add("LEAVEDOOR");

            if (!status.Contains(bookingHeader.Status.ToUpper()))
            {

                throw new Exception("Please check booking status. \r\n โปรดตรวจสอบสถานะของหมายเลขนัดหมายนี้อีกครั้ง");
            }


            if (bookingDetail != null)
            {
                
                // check out PO
                bookingDetail.Status = "CHECKOUT";
                bookingDetail.CheckOut = DateTime.Now;

                unitOfWork.BookingDetailRepository.Update(bookingDetail);

                unitOfWork.Save();

                bookingDetails = new List<BookingDetail>();

                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                int totalCheckIn = bookingDetails.Count(x => x.Status.ToUpper() == "CHECKOUT");

                
                if (totalCheckIn == bookingDetails.Count())
                {
                    bookingHeader.Status = "COMPLETED";
                    bookingHeader.ModDate = DateTime.Now;

                    var checkInTrucks = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                    DateTime timeStamp = DateTime.Now;
                    foreach (var truck in checkInTrucks)
                    {
                        truck.DepartureTime = timeStamp;
                        unitOfWork.BookingTruckCheckInRepository.Update(truck);
                    }
                    unitOfWork.Save();
                }
                else
                {
                    bookingHeader.Status = "CHECKOUT_PROCESS";
                    bookingHeader.ModDate = DateTime.Now;
                }

                unitOfWork.BookingHeaderRepository.Update(bookingHeader);
                unitOfWork.Save();

                //bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingDetail.InternalHeaderKey.Value);
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }

            List<string> statuses = new List<string>();

            statuses.Add("NEW");
            statuses.Add("CHECKIN_PROCESS");

            if (statuses.Contains(bookingHeader.Status))
            {
                throw new Exception("This booking cannot check-out" + "\r\n" + "หมายเลขนัดหมายนี้ไม่สามารถทำการ check-out ได้");
            }

            if (bookingHeader.InternalSupGroupId == sup.InternalGroupId)
            {
                bookingHeaderDto.InternalHeaderKey = bookingHeader.InternalHeaderKey;
                bookingHeaderDto.SupCode = bookingHeader.SupCode;
                bookingHeaderDto.WarehouseCode = bookingHeader.WarehouseCode;
                bookingHeaderDto.BookingId = bookingHeader.BookingId;
                bookingHeaderDto.InternalDoorId = bookingHeader.InternalDoorId.Value;
                bookingHeaderDto.DockDoor = bookingHeader.InternalDoorId > 0 ? unitOfWork.DoorRepository.GetDoorById(bookingHeader.InternalDoorId.Value).DoorName : "-";
                bookingHeaderDto.BookingStart = bookingHeader.BookingStart.Value;
                bookingHeaderDto.BookingEnd = bookingHeader.BookingEnd.Value;

                truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

                // get booking detail
                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                // get booking truck
                bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                var bookingTruckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingDetail.InternalHeaderKey.Value).ToList();

                bookingTruckCheckIns.ForEach((truckCheckIn) =>
                {
                    BookingTruckCheckInDto checkInDto = new BookingTruckCheckInDto();
                    checkInDto.InternalTruckCheckInId = Convert.ToInt32(truckCheckIn.InternalTruckCheckInId);
                    checkInDto.InternalTruckId = Convert.ToInt32(truckCheckIn.InternalTruckId);
                    checkInDto.DriverName = truckCheckIn.DriverName;
                    checkInDto.LicensePlate = truckCheckIn.LicensePlate;
                    checkInDto.LicensePlate2 = truckCheckIn.LicensePlate2;
                    checkInDto.TelNo = truckCheckIn.TelNo;
                    checkInDto.BookingTruckCheckInDetails = new List<BookingTruckCheckInDetail>();

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
                bookingHeaderDto.BookingCheckIns = new List<BookingTruckCheckInDto>();
                bookingHeaderDto.BookingCheckIns = bookingCheckIns;

                return bookingHeaderDto;
            }
            else
            {
                throw new Exception("Incorrect PO or PO isn't booked" + "\r\n" + "หมายเลขใบสั่งซื้อผิดหรือยังไม่ได้ถูกนัดหมาย");
            }
        }

        public async Task<List<BookingTruckCheckInDto>> SavePreCheckIn(PreCheckIn preCheckIn)
        {
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            BookingHeader bookingHeader = new BookingHeader();
            BookingTruckCheckIn bookingCheckIn = new BookingTruckCheckIn();
            BookingTruckCheckInDetail bookingCheckInDetail = new BookingTruckCheckInDetail();
            
            List<BookingTruckCheckInDto> result = new List<BookingTruckCheckInDto>();

            // get booking header
            bookingHeader = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(preCheckIn.InternalHeaderKey);
            bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(preCheckIn.InternalHeaderKey).ToList();

            // check duplicate license plate
            foreach (var item in preCheckIn.BookingTruckCheckIns)
            {
                var licensePlate = item.LicensePlate + "|" + (item.LicensePlate2 == null ? "" : item.LicensePlate2);
                var checkDup = unitOfWork.PoListRepository.CheckDuplicateTruck(bookingHeader.BookingStart.Value, bookingHeader.BookingId, licensePlate);
                if(checkDup > 0)
                {
                    throw new Exception("License plate " + licensePlate + " is duplicate in same booking time" + "\r\n" + "พบมีการจองทะเบียนซ้ำในช่วงเวลาเดียวกัน, กรุณาติดต่อทีม Booking");
                }
            }

            var truckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(preCheckIn.InternalHeaderKey);

            foreach (var item in truckCheckIns)
            {                
                unitOfWork.BookingTruckCheckInDetailRepository.Remove(item.InternalTruckCheckInId);
                unitOfWork.BookingTruckCheckInRepository.Remove(item.InternalTruckCheckInId);
            }

            unitOfWork.Save();

            bookingHeader.Status = "INTRANSIT";
            bookingHeader.UserStamp = preCheckIn.BookingTruckCheckIns[0].UserStamp;
            bookingHeader.ModDate = DateTime.Now;
            
            foreach (var checkIn in preCheckIn.BookingTruckCheckIns)
            {
                var checkInId = unitOfWork.PoListRepository.GetBookingTruckCheckInKey();
                unitOfWork.Save();

                bookingCheckIn = new BookingTruckCheckIn();
                bookingCheckIn.InternalTruckCheckInId = checkInId;
                bookingCheckIn.InternalHeaderKey = preCheckIn.InternalHeaderKey;
                bookingCheckIn.InternalTruckId = checkIn.InternalTruckId;
                bookingCheckIn.LicensePlate = checkIn.LicensePlate.Trim();
                bookingCheckIn.LicensePlate2 = checkIn.LicensePlate2;
                bookingCheckIn.DriverName= checkIn.DriverName.Trim();
                bookingCheckIn.TelNo = checkIn.TelNo;
                bookingCheckIn.LineId = checkIn.LineId;
                bookingCheckIn.CheckInTime = DateTime.Now;
                bookingCheckIn.Status = "INTRANSIT";
                bookingCheckIn.UserStamp = checkIn.UserStamp;

                unitOfWork.BookingTruckCheckInRepository.Add(bookingCheckIn);

                unitOfWork.Save();

                // create booking checkin detail
                foreach (var checkInDtl in checkIn.BookingTruckCheckInDetails)
                {
                    var bkDtl = unitOfWork.BookingDetailRepository.GetBookingDetailByPoNoAndId(checkInDtl.PoNbr, preCheckIn.InternalHeaderKey);

                    var checkInDtlId = unitOfWork.PoListRepository.GetBookingTruckCheckInDtlKey();
                    
                    unitOfWork.Save();

                    bookingCheckInDetail = new BookingTruckCheckInDetail();
                    bookingCheckInDetail = checkInDtl;
                    bookingCheckInDetail.InternalTruckCheckInId = bookingCheckIn.InternalTruckCheckInId;
                    bookingCheckInDetail.InternalTruckDetailId = checkInDtlId;

                    bookingCheckInDetail.InternalDetailKey = bkDtl.InternalDetailKey;

                    unitOfWork.BookingTruckCheckInDetailRepository.Add(bookingCheckInDetail);

                    unitOfWork.Save();
                }

                // save log
                BookingTruckLog truckLog = new BookingTruckLog();
                truckLog.Action = "CREATE";
                truckLog.Remark = "";
                truckLog.UserStamp = preCheckIn.BookingTruckCheckIns[0].UserStamp;
                truckLog.DateTimeStamp = DateTime.Now;
                truckLog.InternalTruckCheckInId = bookingCheckIn.InternalTruckCheckInId;

                unitOfWork.BookingTruckLogRepository.Add(truckLog);
                unitOfWork.Save();
            }

            unitOfWork.BookingHeaderRepository.Update(bookingHeader);

            var bookingInterface = new BookingInterface();
            bookingInterface.BookingId = bookingHeader.BookingId;
            bookingInterface.IsInterface = false;
            bookingInterface.DateTimeStamp = DateTime.Now;
            bookingInterface.Status =bookingHeader.Status;
            bookingInterface.UserStamp = preCheckIn.BookingTruckCheckIns[0].UserStamp;

            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);


            bookingDetails.ForEach(bkDtl => {
                bkDtl.PreCheckIn = DateTime.Now;
                bkDtl.Status = "INTRANSIT";
                bkDtl.UserStamp = preCheckIn.BookingTruckCheckIns[0].UserStamp;
                bkDtl.ModDate = DateTime.Now;

                unitOfWork.BookingDetailRepository.Update(bkDtl);
            });

            unitOfWork.Save();

            return null;
        }

        public async Task<List<BookingCheckInDto>> CheckInPo(CheckInDto checkIn)
        {

            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByPoNo(checkIn.PoNbr);

            if(bookingDtls == null || bookingDtls.Count == 0)
            {
                throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
            }

            for (int i = 0; i < bookingDtls.Count(); i++)
            {
                bookingDtls[i].Status = "CHECKIN";
                bookingDtls[i].DocumentCheckIn = DateTime.Now;
                bookingDtls[i].ModDate= DateTime.Now;
                bookingDtls[i].UserStamp = checkIn.UserStamp;

                unitOfWork.BookingDetailRepository.Update(bookingDtls[i]);
            }

            unitOfWork.Save();
            
            return await this.GetBookingCheckIn(checkIn.SupCode, "123");
           
            //return null;
        }

        public async Task<List<BookingCheckInDto>> CheckInBooking(CheckInDto checkIn)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(checkIn.PoNbr);

            if(bookingHdr == null)
            {
                throw new Exception("Booking Id not found" + "\r\n" + "ไม่พบหมายเลขนัดหมาย");
            }

            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHdr.InternalHeaderKey).ToList();

            if (bookingDtls == null || bookingDtls.Count == 0)
            {
                throw new Exception("Not found PO in this booking id" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อในการนัดหมาย");
            }

            for (int i = 0; i < bookingDtls.Count(); i++)
            {
                bookingDtls[i].Status = "CHECKIN";
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

            return await this.GetBookingCheckIn(checkIn.SupCode, "123");

            //return null;
        }

        public async Task<bool> DeleteBookingHeader(int bookingHeaderId,string userName)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderId);

            if (bookingHdr == null)
            {
                throw new Exception("Booking No Not found" + "\r\n" + "ไม่พบหมายเลข booking ในระบบ");
            }

            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeaderId).ToList();

            unitOfWork.BookingHeaderRepository.Remove(bookingHdr);

            foreach (var bookingDtl in bookingDtls)
            {
                unitOfWork.BookingDetailRepository.Remove(bookingDtl);
            }
            
            var bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingHeaderId).ToList();

            foreach (var bookingTruck in bookingTrucks)
            {
                unitOfWork.BookingTruckRepository.Remove(bookingTruck);
            }

            var bookingInterface = new BookingInterface();
            bookingInterface.BookingId = bookingHdr.BookingId;
            bookingInterface.IsInterface = false;
            bookingInterface.DateTimeStamp = DateTime.Now;
            bookingInterface.Status = "DELETED";
            bookingInterface.UserStamp = userName;

            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

            unitOfWork.Save();

            return true;

            //return null;
        }

        public async Task<bool> DeleteBookingHeaderAll(int bookingHeaderId,string userName)
        {
            
            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderId);

            var bookingKey = unitOfWork.BookingKeyRepository.GetBookingKeyById(bookingHdr.InternalKeyId.Value);

            var bookingHdrs = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingHdr.InternalKeyId.Value);

            if (bookingHdr == null)
            {
                throw new Exception("Booking No Not found" + "\r\n" + "ไม่พบหมายเลข booking ในระบบ");
            }

            foreach (var bookHdr in bookingHdrs)
            {
                // get booking detail
                var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookHdr.InternalHeaderKey).ToList();

                foreach (var bookingDtl in bookingDtls)
                {
                    unitOfWork.BookingDetailRepository.Remove(bookingDtl);
                }

                var bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookHdr.InternalHeaderKey).ToList();

                foreach (var bookingTruck in bookingTrucks)
                {
                    unitOfWork.BookingTruckRepository.Remove(bookingTruck);
                }

                unitOfWork.BookingHeaderRepository.Remove(bookHdr);

                var bookingInterface = new BookingInterface();
                bookingInterface.BookingId = bookHdr.BookingId;
                bookingInterface.IsInterface = false;
                bookingInterface.DateTimeStamp = DateTime.Now;
                bookingInterface.Status = "DELETED";
                bookingInterface.UserStamp = userName;

                unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

            }

            unitOfWork.BookingKeyRepository.Remove(bookingKey);

            unitOfWork.Save();

            return true;

            //return null;
        }

        public async Task<bool> DeletePreCheckIn(int bookingHeaderId, string userName)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderId);

            if (bookingHdr == null)
            {
                throw new Exception("Booking No Not found" + "\r\n" + "ไม่พบหมายเลข booking ในระบบ");
            }

            // get booking detail
            var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHeaderId).ToList();

            bookingHdr.Status = "APPROVED";

            foreach (var bookingDtl in bookingDtls)
            {
                bookingDtl.Status = "NEW";
            }

            // remove truck checkin
            var bookingCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHeaderId).ToList();

            foreach (var bookingCheckIn in bookingCheckIns)
            {
                var bookingCheckInDtls = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(bookingCheckIn.InternalTruckCheckInId)).ToList();
                foreach (var bookingDtl in bookingCheckInDtls)
                {
                    unitOfWork.BookingTruckCheckInDetailRepository.Remove(bookingDtl);
                }
                unitOfWork.BookingTruckCheckInRepository.Remove(bookingCheckIn);
            }

            var bookingInterface = new BookingInterface();
            bookingInterface.BookingId = bookingHdr.BookingId;
            bookingInterface.IsInterface = false;
            bookingInterface.DateTimeStamp = DateTime.Now;
            bookingInterface.Status =bookingHdr.Status;
            bookingInterface.UserStamp = userName;

            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

            unitOfWork.Save();

            return true;

            //return null;
        }

        public async Task<bool> ApprovedBooking(int bookingHeaderId, string userName)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderId);

            if (bookingHdr == null)
            {
                throw new Exception("Booking No Not found" + "\r\n" + "ไม่พบหมายเลข booking ในระบบ");
            }

            bookingHdr.Status = "APPROVED";

            unitOfWork.BookingHeaderRepository.Update(bookingHdr);

            var bookingInterface = new BookingInterface();
            bookingInterface.BookingId = bookingHdr.BookingId;
            bookingInterface.IsInterface = false;
            bookingInterface.DateTimeStamp = DateTime.Now;
            bookingInterface.Status =bookingHdr.Status;
            bookingInterface.UserStamp = userName;

            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

            unitOfWork.Save();

            try
            {
                mailSend(bookingHdr);
            }
            catch(Exception ex)
            {

            }

            return true;

        }

        public async Task<BookingHeaderDto> GetBookingByBookingId(string bookingId)
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

            statuses.Add("CHECKIN_COMPLETED");
            statuses.Add("CHECKOUT_PROCESS");
            statuses.Add("COMPLETED");


            if (bookingHeader.Status.ToUpper() == "NEW" || bookingHeader.Status.ToUpper() == "OVERCAP")
            {
                //throw new Exception("This booking on the approve process");
                throw new Exception("Booking Id on the approve process" + "\r\n" + "หมายเลขนัดหมายกำลังอยู่ในขั้นตอนการพิจารณา");
            }

            //if (statuses.Contains(bookingHeader.Status))
            //{
            //    throw new Exception("Booking Id already check-in" + "\r\n" + "หมายเลขนัดหมายถูก check-in แล้ว");
            //}

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
            bookingHeaderDto.RemarkDelay = bookingHeader.RemarkDelay;

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
                checkInDto.DriverName = truckCheckIn.DriverName.Trim();
                checkInDto.LicensePlate = truckCheckIn.LicensePlate.Trim();
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

        public async Task<List<BookingTruckLog>> GetBookingLog(int internalTruckId)
        {
            return unitOfWork.BookingTruckLogRepository.GetBookingTruckLogByInternalTruckCheckinId(internalTruckId).OrderBy(x=>x.DateTimeStamp).ToList();
        }

        public async Task<bool> BackHaulBooking(int bookingHeaderId,bool isBackHaul,string userName)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderId);

            if (bookingHdr == null)
            {
                throw new Exception("Booking No Not found" + "\r\n" + "ไม่พบหมายเลข booking ในระบบ");
            }

            if(bookingHdr.BackHaul != isBackHaul)
            {
                bookingHdr.BackHaul = isBackHaul;
                bookingHdr.UserStamp = userName;
                bookingHdr.ModDate = DateTime.Now;

                unitOfWork.BookingHeaderRepository.Update(bookingHdr);

                var bookingInterface = new BookingInterface();
                bookingInterface.BookingId = bookingHdr.BookingId;
                bookingInterface.IsInterface = false;
                bookingInterface.DateTimeStamp = DateTime.Now;
                bookingInterface.Status = bookingHdr.Status;
                bookingInterface.UserStamp = userName;

                unitOfWork.BookingInterfaceRepository.Add(bookingInterface);


                unitOfWork.Save();
            }


            return true;

        }

        public async Task<bool> DcDelayBooking(int bookingHeaderId, List<BookingDetail> bookingDetails, string userName)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHeaderId);

            if (bookingHdr == null)
            {
                throw new Exception("Booking No Not found" + "\r\n" + "ไม่พบหมายเลข booking ในระบบ");
            }

            foreach (var bd in bookingDetails)
            {
                if (bd.IsDelay.Value)
                {
                    var bookingDtl = unitOfWork.BookingDetailRepository.GetBookingDetailById(bd.InternalDetailKey);
                    if (bookingDtl != null)
                    {
                        if (bookingDtl.DelayReason != bd.DelayReason)
                        {
                            bookingDtl.DelayReason = bd.DelayReason;
                            bookingDtl.UserStamp = userName;
                            bookingDtl.ModDate = DateTime.Now;

                            unitOfWork.BookingDetailRepository.Update(bookingDtl);
                            unitOfWork.PoListRepository.UpdatePoPostponed(bookingDtl.PoNbr, bookingDtl.DelayReason);

                            PoLog poLog = new PoLog();
                            poLog.UserStamp = userName;
                            poLog.DateTimeStamp = DateTime.Now;
                            poLog.PoNbr = bookingDtl.PoNbr;
                            poLog.Log = bookingDtl.DelayReason;
                            unitOfWork.PoLogRepository.Add(poLog);

                            unitOfWork.Save();
                        }
                    }                    
                }                
            }

            var bookingInterface = new BookingInterface();
            bookingInterface.BookingId = bookingHdr.BookingId;
            bookingInterface.IsInterface = false;
            bookingInterface.DateTimeStamp = DateTime.Now;
            bookingInterface.Status = bookingHdr.Status;
            bookingInterface.UserStamp = userName;

            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);
            unitOfWork.Save();

            return true;

        }


        public async Task<bool> CancelBooking(BookingHeaderDto bookingHeaderDto,string userName)
        {

            var bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingHeaderDto.BookingId);

            if (bookingHdr == null)
            {
                throw new Exception("Booking Id not found" + "\r\n" + "ไม่พบหมายเลขนัดหมาย");
            }

            bookingHdr.Status = "CANCEL";
            bookingHdr.UserStamp = userName;
            bookingHdr.ModDate = DateTime.Now;
            bookingHdr.UserCancel = userName;
            bookingHdr.RemarkCancel = bookingHeaderDto.RemarkCancel;

            unitOfWork.BookingHeaderRepository.Update(bookingHdr);

            var bookingInterface = new BookingInterface();
            bookingInterface.BookingId = bookingHdr.BookingId;
            bookingInterface.IsInterface = false;
            bookingInterface.DateTimeStamp = DateTime.Now;
            bookingInterface.Status = bookingHdr.Status;
            bookingInterface.UserStamp = userName;

            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

            unitOfWork.Save();

            mailCancel(bookingHdr);

            return true;
        }

        private void mailSend(BookingHeader bookingHdr)
        {
            string htmlContent = @"
                <!DOCTYPE html>
                <html lang='th'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Notification Email</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;'>
                    <table style='max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='text-align: left;'>
                                <h2 style='color: #4caf50;'>เรียน ซัพพลายเออร์</h2>
                                <p style='color: #333;text-indent: 2em;'>
                                    ทีม Booking ได้ทำการอนุมัติ Booking ที่จองไว้ให้เรียบร้อยแล้ว 
                                    สามารถตรวจสอบข้อมูลในระบบ <strong>IMS</strong> ได้เลยค่ะ
                                </p>
                                <p style='color: #333;text-indent: 2em;'>
                                    หากจัดเตรียมสินค้าขึ้นรถเพื่อทำการจัดส่ง ให้ทำการแจ้งข้อมูลพนักงานขับรถในระบบ 
                                    <strong>IMS</strong> ผ่านเมนู <strong>Assign Truck</strong> ก่อนสั่งพิมพ์เอกสาร 
                                    <strong>Gate Pass</strong> ให้กับพนักงานขับรถ หรือแก้ไขข้อมูลก่อนรถขนส่งเดินทางถึงคลังสินค้าด้วยค่ะ
                                </p>
                                <p style='color: #333; font-weight: bold;'>
                                    หมายเลข Booking ID : <span style='color: #4caf50;'>{0}</span>
                                </p>
                                <p style='color: #333;'>
                                    ขอบคุณค่ะ<br>
                                    <strong>Booking Team</strong>
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";


            
            if (bookingHdr.Status == "APPROVED")
            {
                //  var supGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingHdr.InternalSupGroupId.Value);

                if (bookingHdr.ContactEmail.Contains("@"))
                {
                    Helper.Email.SendEmail(bookingHdr.ContactEmail, string.Format(htmlContent, bookingHdr.BookingId), "Makro : IMS Booking Approvaled");
                }

            }
            
        }

        private void mailCancel(BookingHeader bookingHdr)
        {
            string htmlContent = @"
                <!DOCTYPE html>
                <html lang='th'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Notification Email</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;'>
                    <table style='max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='text-align: left;'>
                                <h2 style='color: red'>เรียน ซัพพลายเออร์</h2>
                                <p style='color: #333;text-indent: 2em;'>
                                    ทีม Booking ได้ทำการยกเลิกรอบการจัดส่งของท่าน สามารถทำการ booking ใหม่เพื่อทำการเลือกวันและเวลาที่จัดส่งใหม่ที่ต้องการได้เลยคะ
                                </p>
                                <p style='color: #333;text-indent: 2em;'>
                                    หากติดปัญหาไม่สามารถจัดส่งได้ตามวันและเวลาดังกล่าว กรุณาติดต่อ Booking คะ                                     
                                </p>
                                <p style='color: #333; font-weight: bold;'>
                                    หมายเลข Booking ID : <span style='color: red;'>{0}</span> <br>                                    
                                </p>
                                <p style='color: #333; font-weight: bold;'>
                                    *หมายเหตุ : {1}
                                </p>
                                <p style='color: #333;'>
                                    ขอบคุณค่ะ<br>
                                    <strong>Booking Team</strong>
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                var mail = bookingHdr.ContactEmail;
                
                if (mail.Contains("@"))
                {
                    Helper.Email.SendEmail(mail, string.Format(htmlContent, bookingHdr.BookingId, bookingHdr.RemarkCancel), "Makro : IMS Booking ยกเลิกรอบจัดส่ง");
                }
            
        }


    }
}
