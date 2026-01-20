using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Makro.IMS.Services.Api.Services
{
    public class BookingKeyService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private WarehouseCapacityService wcs;


        public BookingKeyService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            wcs = new WarehouseCapacityService(null);
        }

        public async Task<List<BookingKey>> GetBookingsAsync()
        {
            return await Task.Run<List<BookingKey>>(() =>  unitOfWork.BookingKeyRepository.GetBookingKeys().ToList());
        }

        public async Task<BookingKey> GetBookingsByIdAsync(int keyId)
        {
            return await Task.Run<BookingKey>(() => unitOfWork.BookingKeyRepository.GetBookingKeyById(keyId)!);
        }

        public async Task<List<BookingHeader>> GetBookingHeaderBySupCodeAndBookingDate(string supCode,DateTime bookingDate)
        {
            return await Task.Run<List<BookingHeader>>(
                () => unitOfWork.BookingHeaderRepository.GetBookingHeaderBySupplierAndBookingDate(supCode, bookingDate).ToList()!
                );
        }


        public async Task<List<string>> SaveBooking(BookingKeyDto bookingKeyDto,string createBy)
        {
            BookingKey bookingKey = new BookingKey();
            BookingHeader bookingHdr = new BookingHeader();
            BookingDetail bookingDtl = new BookingDetail();
            BookingTruck bookingTruck = new BookingTruck();
            WarehouseCapacity warehouseCapacity = new WarehouseCapacity();
            Warehouse warehouse = new Warehouse();
            
            SupplierGroup supGroup = new SupplierGroup();
            bool result = false;
            List<string> strResult = new List<string>();

            List<BookingHeader> bookingCreateList = new List<BookingHeader>();

            Operation operation = new Operation();

            string bookingBatch = "";
            try
            {
                
                if (this.CheckCapBooking(ref bookingKeyDto))
                {
                    if (bookingKeyDto.BookingHeaders.Count(x => x.Status == "FAIL") > 0)
                    {
                        var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(bookingKeyDto.UserName);
                        if (user != null && (user.UserType.ToUpper() == "SUP" || user.UserType.ToUpper() == "BH" || user.UserType.ToUpper() == "SUPTRAN") && createBy != "IMPORT")
                        {
                            throw new Exception("Save failed. Over capacity : ไม่สามารถบันทึกข้อมูลได้เนื่องจากเกิน capacity ที่กำหนด");
                        }
                        else
                        {
                            bookingKeyDto.BookingHeaders.ForEach(x => x.Status = x.Status == "FAIL" ? "OVERCAP" : x.Status);
                        }
                    }
                }

                // get supplier data
                var supgroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingKeyDto.BookingHeaders[0].InternalSupGroupId);

                // validate duplicate slot booking
                foreach (var bookingHdrDto in bookingKeyDto.BookingHeaders)
                {
                    if (bookingHdrDto.InternalDoorId != 0)
                    {
                        var dupSlot = unitOfWork.BookingHeaderRepository.GetBookingHeaderByDoorAndSlotTime(bookingHdrDto.InternalDoorId, bookingHdrDto.BookingStart, bookingHdrDto.BookingEnd);

                        if (dupSlot != null)
                        {
                            throw new Exception("Duplicate slot time please check and booking again");
                        }
                    }
                }

                //var bookingKeyId = unitOfWork.PoListRepository.GetBookingKey();
                //unitOfWork.Save();

                //bookingKey.InternalKeyId = bookingKeyId;
                //bookingKey.BookingDate = bookingKeyDto.BookingDate.ToLocalTime().Date;
                //bookingKey.CreateDate = DateTime.Now;
                //bookingKey.Active = true;
                //bookingKey.CompanyCode = "88";
                //bookingKey.UserStamp = bookingKeyDto.UserName;
                //bookingKey.ModDate = DateTime.Now;

                //result = await Task.Run<bool>(() => unitOfWork.BookingKeyRepository.Add(bookingKey));
                
                bookingBatch = "";
                
                foreach (var bookingHdrDto in bookingKeyDto.BookingHeaders)
                {
                    if (bookingHdrDto.Status == "FAIL" && createBy != "IMPORT")
                    {
                        bookingHdrDto.BookingId = "";
                    }
                    else
                    {
                        var bookingKeyId = unitOfWork.PoListRepository.GetBookingKey();
                        unitOfWork.Save();

                        if (bookingBatch == "")
                        {
                            bookingBatch = createBy == "IMPORT" ? "I" + bookingKeyId.ToString() : bookingKeyId.ToString();
                        }

                        bookingKey.InternalKeyId = bookingKeyId;
                        bookingKey.BookingDate = bookingKeyDto.BookingDate.ToLocalTime().Date;
                        bookingKey.CreateDate = DateTime.Now;
                        bookingKey.Active = true;
                        bookingKey.CompanyCode = "88";
                        bookingKey.UserStamp = bookingKeyDto.UserName;
                        bookingKey.ModDate = DateTime.Now;

                        result = await Task.Run<bool>(() => unitOfWork.BookingKeyRepository.Add(bookingKey));

                        string bookingId = "";
                        string status = "NEW";
                        string approvedCondition = "";

                        // generate booking header id
                        warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(bookingHdrDto.WarehouseCode);
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

                        // count booking truck 
                        if ((bookingHdrDto.BookingTrucks.Sum(x => x.TotalTruck) > 1) || (bookingHdrDto.BookingTrucks.Count > 1))
                        {
                            status = "NEW";
                        }
                        else // auto approved
                        {
                            status = "APPROVED";
                        }

                        //if(createBy == "IMPORT")
                        //{
                        //    status = "NEW";
                        //}

                        if (warehouse.CapUom.ToUpper() == "CS")
                        {
                            if (bookingHdrDto.BookingDetails.Any(x => x.PlanRec?.Date < bookingKey.BookingDate?.Date))
                            {
                                status = "NEW";
                            }
                        }

                        if (bookingHdrDto.Status != null  && (bookingHdrDto.Status == "OVERCAP" || bookingHdrDto.Status == "OVERCUTOFF"))
                        {
                            status = bookingHdrDto.Status;
                            approvedCondition = status;
                            //status = "OVERCAP";
                        }

                        bookingId = warehouse.OnlineBookingIdPrefix + DateTime.Now.Date.ToString("MMdd")
                                + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');

                        bookingHdr = new BookingHeader();
                        bookingHdr.InternalHeaderKey = unitOfWork.PoListRepository.GetBookingHeaderKey();
                        bookingHdr.InternalKeyId = bookingKey.InternalKeyId;
                        bookingHdr.InternalSupGroupId = bookingHdrDto.InternalSupGroupId;
                        bookingHdr.InternalDoorId = bookingHdrDto.InternalDoorId;
                        bookingHdr.BookingId = bookingId;
                        bookingHdr.BookingStart = bookingHdrDto.BookingStart.ToLocalTime();
                        bookingHdr.BookingEnd = bookingHdrDto.BookingEnd.ToLocalTime();
                        bookingHdr.FirstBookginStart = bookingHdrDto.FirstBookingStart.ToLocalTime();
                        bookingHdr.FirstBookingEnd = bookingHdrDto.FirstBookingEnd.ToLocalTime();
                        bookingHdr.ContactName = bookingHdrDto.ContactName;
                        bookingHdr.ContactEmail = bookingHdrDto.ContactEmail;
                        bookingHdr.ContactTel = bookingHdrDto.ContactTel;
                        bookingHdr.WarehouseCode = bookingHdrDto.WarehouseCode;
                        bookingHdr.SupCode = bookingHdrDto.SupCode;
                        bookingHdr.SupName = bookingHdrDto.SupName;
                        bookingHdr.TotalPo = bookingHdrDto.BookingDetails.Count();
                        bookingHdr.TotalQty = bookingHdrDto.BookingDetails.Sum(x => x.TotalQty);
                        bookingHdr.Active = true;
                        bookingHdr.Status = status;
                        bookingHdr.UserStamp = bookingKeyDto.UserName;
                        bookingHdr.FirstUserStamp = bookingKeyDto.UserName;
                        bookingHdr.CreateDate = DateTime.Now;
                        bookingHdr.Remark = bookingHdrDto.Remark;
                        bookingHdr.ModDate = DateTime.Now;
                        bookingHdr.CompanyCode = bookingHdrDto.CompanyCode;
                        bookingHdr.MerchType = bookingHdrDto.MerchType;
                        bookingHdr.OriginalMerchType = bookingHdrDto.MerchType;
                        bookingHdr.IsDelay = bookingHdrDto.BookingDetails.Count(x => x.IsDelay.Value == true) > 0 ? true : false;
                        bookingHdr.Postponed = bookingHdrDto.PostPoned;
                        bookingHdr.BackHaul = bookingHdrDto.BackHaul;
                        bookingHdr.RemarkDelay = bookingHdrDto.RemarkDelay;
                        bookingHdr.ApproveCondition = approvedCondition; //status == "OVERCAP" ? "OVER CAP" : "";
                        bookingHdr.RevisionPrefix = bookingBatch;

                        var whseBookingCap = await wcs.GetOperationCapacityByBookingDate(bookingKey.BookingDate.Value, bookingHdr.WarehouseCode, bookingHdr.MerchType);

                        var checkCaps = whseBookingCap.Where(x => x.BookingDateTime >= bookingHdr.BookingStart && x.BookingDateTime <= bookingHdr.BookingEnd);

                        unitOfWork.BookingHeaderRepository.Add(bookingHdr);

                        bookingCreateList.Add(bookingHdr);

                        var bookingInterface = new BookingInterface();
                        bookingInterface.BookingId = bookingHdr.BookingId;
                        bookingInterface.IsInterface = false;
                        bookingInterface.DateTimeStamp = DateTime.Now;
                        bookingInterface.Status = bookingHdr.Status;
                        bookingInterface.UserStamp = bookingKeyDto.UserName;

                        unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

                        unitOfWork.Save();

                        strResult.Add(bookingHdr.WarehouseCode + " : " + bookingId);

                        foreach (var bookingDtlDto in bookingHdrDto.BookingDetails)
                        {
                            var detailId = unitOfWork.PoListRepository.GetBookingDetailKey();
                            unitOfWork.Save();

                            bookingDtl = new BookingDetail();
                            bookingDtl = bookingDtlDto;
                            bookingDtl.InternalDetailKey = detailId;
                            bookingDtl.InternalHeaderKey = bookingHdr.InternalHeaderKey;
                            bookingDtl.UserStamp = bookingKeyDto.UserName;
                            bookingDtl.Status = "NEW";
                            bookingDtl.CreateDate = DateTime.Now;
                            bookingDtl.ModDate = DateTime.Now;

                            unitOfWork.BookingDetailRepository.Add(bookingDtl);

                            if (bookingDtlDto.IsDelay.Value == true)
                            {
                                unitOfWork.PoListRepository.UpdatePoPostponed(bookingDtlDto.PoNbr, bookingDtlDto.DelayReason);
                                PoLog poLog = new PoLog();
                                poLog.UserStamp = bookingKeyDto.UserName;
                                poLog.DateTimeStamp = DateTime.Now;
                                poLog.PoNbr = bookingDtl.PoNbr;
                                poLog.Log = bookingDtl.DelayReason;
                                unitOfWork.PoLogRepository.Add(poLog);

                            }

                            unitOfWork.Save();
                        }

                        foreach (var bookingTruckDto in bookingHdrDto.BookingTrucks)
                        {
                            bookingTruck = new BookingTruck();
                            bookingTruck = bookingTruckDto;
                            bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;
                            unitOfWork.BookingTruckRepository.Add(bookingTruck);
                        }
                    }
                }

                unitOfWork.Save();

                // send approved mail
                try
                {
                    // send mail for booking approved.
                    mailSend(bookingCreateList);
                }
                catch (Exception ex)
                {
                }
            }
            catch(Exception ex)
            {
                // rollback transaction                
                throw new Exception(ex.Message);
            }
            return strResult;
        }

        public async Task<List<string>> UpdateBooking(BookingKeyDto bookingKeyDto,string userName)
        {
            BookingKey bookingKey = new BookingKey();
            BookingHeader bookingHdr = new BookingHeader();
            BookingDetail bookingDtl = new BookingDetail();
            BookingTruck bookingTruck = new BookingTruck();
            WarehouseCapacity warehouseCapacity = new WarehouseCapacity();
            Warehouse warehouse = new Warehouse();

            SupplierGroup supGroup = new SupplierGroup();
            bool result = false;
            List<string> strResult = new List<string>();

            List<BookingHeader> bookingCreateList = new List<BookingHeader>();

            bool isSlottimeChange = false;

            Dictionary<string, string> bookingChange = new Dictionary<string, string>();

            UserMaster user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(bookingKeyDto.UserName);

            try
            {

                if (bookingKeyDto.BookingHeaders.Count == 0)
                {
                    bookingKey = unitOfWork.BookingKeyRepository.GetBookingKeyById(bookingKeyDto.InternalKeyId.Value);

                    var bookingHdrs = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingKeyDto.InternalKeyId.Value);


                    foreach (var bookHdr in bookingHdrs)
                    {
                        // get booking detail
                        var bookingDtls = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookHdr.InternalHeaderKey).ToList();

                        foreach (var bookDtl in bookingDtls)
                        {
                            unitOfWork.BookingDetailRepository.Remove(bookDtl);
                        }

                        var bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookHdr.InternalHeaderKey).ToList();

                        foreach (var bookTruck in bookingTrucks)
                        {
                            unitOfWork.BookingTruckRepository.Remove(bookTruck);
                        }


                        var bookingInterface = new BookingInterface();
                        bookingInterface.BookingId = bookHdr.BookingId;
                        bookingInterface.IsInterface = false;
                        bookingInterface.DateTimeStamp = DateTime.Now;
                        bookingInterface.Status = "DELETED";
                        bookingInterface.UserStamp = userName;

                        unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

                        unitOfWork.Save();

                        unitOfWork.BookingHeaderRepository.Remove(bookHdr);

                    }

                    unitOfWork.BookingKeyRepository.Remove(bookingKey);

                    unitOfWork.Save();

                    strResult.Add("DELETED");
                }
                else
                {
                    strResult.Add("UPDATED");
                    if (this.CheckCapBooking(ref bookingKeyDto) == false)
                    {
                       // var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(bookingKeyDto.UserName);
                        if (user != null && (user.UserType.ToUpper() == "SUP" || user.UserType.ToUpper() == "BH" || user.UserType.ToUpper() == "SUPTRAN"))
                        {
                            throw new Exception("Update failed. Over capacity : ไม่สามารถ update ข้อมูลได้เนื่องจากเกิน capacity ที่กำหนด");
                        }
                    }

                    // get supplier data
                    var supgroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingKeyDto.BookingHeaders[0].InternalSupGroupId);

                    // validate duplicate slot booking
                    foreach (var bookingHdrDto in bookingKeyDto.BookingHeaders)
                    {
                        if (bookingHdrDto.InternalDoorId != 0)
                        {
                            var dupSlot = unitOfWork.BookingHeaderRepository.GetBookingHeaderByDoorAndSlotTime(bookingHdrDto.InternalDoorId, bookingHdrDto.BookingStart, bookingHdrDto.BookingEnd);

                            if (dupSlot != null)
                            {
                                throw new Exception("Duplicate slot time please check and booking again");
                            }
                        }
                    }

                    bookingKey = unitOfWork.BookingKeyRepository.GetBookingKeyById(bookingKeyDto.InternalKeyId.Value);

                    bookingKey.BookingDate = bookingKeyDto.BookingDate.ToLocalTime().Date;
                    bookingKey.Active = true;
                    bookingKey.CompanyCode = "88";
                    bookingKey.UserStamp = bookingKeyDto.UserName;
                    bookingKey.ModDate = DateTime.Now;

                    result = await Task.Run<bool>(() => unitOfWork.BookingKeyRepository.Update(bookingKey));

                    unitOfWork.Save();

                    // get list booking id of current data
                    var bookingIds = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingKeyDto.InternalKeyId.Value).ToList();

                    // booking id list of update data
                    var bookingIdList = bookingKeyDto.BookingHeaders.Select(x => x.BookingId).ToList();

                    // remove booking not in update data
                    var bookingIdsDelete = bookingIds.Where(x => !bookingIdList.Contains(x.BookingId)).ToList();

                    // remove booking id
                    foreach (var bookingIdDel in bookingIdsDelete)
                    {
                        // remove truck
                        var bookingTruckDel = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingIdDel.InternalHeaderKey).ToList();

                        if (bookingTruckDel.Count > 0)
                        {
                            unitOfWork.BookingTruckRepository.Remove(bookingTruckDel[0]);
                        }

                        // remove po
                        var bookingPoDels = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingIdDel.InternalHeaderKey).ToList();
                        for (int i = bookingPoDels.Count; i > 0; i--)
                        {
                            unitOfWork.BookingDetailRepository.Remove(bookingPoDels[i - 1]);
                        }
                    }

                    for (int i = bookingIdsDelete.Count; i > 0; i--)
                    {
                        unitOfWork.BookingHeaderRepository.Remove(bookingIdsDelete[i - 1]);

                        var bookingInterface = new BookingInterface();
                        bookingInterface.BookingId = bookingIdsDelete[i-1].BookingId;
                        bookingInterface.IsInterface = false;
                        bookingInterface.DateTimeStamp = DateTime.Now;
                        bookingInterface.Status = "DELETED";
                        bookingInterface.UserStamp = userName;

                        unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

                    }

                    foreach (var bookingHdrDto in bookingKeyDto.BookingHeaders)
                    {
                        string bookingId = "";
                        string status = "EDIT";
                        string approvedCondition = "";
                        
                        warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(bookingHdrDto.WarehouseCode);

                        #region "auto approve"
                        
                        // count booking truck 
                        if ((bookingHdrDto.BookingTrucks.Sum(x => x.TotalTruck) > 1) || (bookingHdrDto.BookingTrucks.Count > 1))
                        {
                            status = "NEW";
                        }
                        else // auto approved
                        {
                            status = "APPROVED";
                        }

                        if (bookingHdrDto.BookingDetails.Any(x => x.PlanRec?.Date < bookingKey.BookingDate?.Date) && warehouse.CapUom.ToUpper() == "CS")
                        {
                            status = "NEW";
                        }

                        if(bookingHdrDto.Status != null && (bookingHdrDto.Status == "OVERCAP" || bookingHdrDto.Status == "OVERCUTOFF"))
                        {
                            status = bookingHdrDto.Status;
                            approvedCondition = status;
                        }

                        #endregion

                        if (bookingHdrDto.InternalHeaderKey.HasValue)
                        {
                            // check update or create
                            bookingHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(bookingHdrDto.InternalHeaderKey.Value);
                        }
                        else
                        {
                            bookingHdr = null;
                        }

                        if (bookingHdr == null)
                        {
                            // generate booking header id
                            warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(bookingHdrDto.WarehouseCode);

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

                            bookingId = warehouse.OnlineBookingIdPrefix + bookingKeyDto.BookingDate.ToLocalTime().Date.ToString("MMdd") + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');

                            bookingHdr = new BookingHeader();
                            bookingHdr.InternalHeaderKey = unitOfWork.PoListRepository.GetBookingHeaderKey();
                            bookingHdr.InternalKeyId = bookingKey.InternalKeyId;
                            bookingHdr.InternalSupGroupId = bookingHdrDto.InternalSupGroupId;
                            bookingHdr.InternalDoorId = bookingHdrDto.InternalDoorId;
                            bookingHdr.BookingId = bookingId;
                            bookingHdr.BookingStart = bookingHdrDto.BookingStart.ToLocalTime();
                            bookingHdr.BookingEnd = bookingHdrDto.BookingEnd.ToLocalTime();
                            bookingHdr.FirstBookginStart = bookingHdrDto.FirstBookingStart.ToLocalTime();
                            bookingHdr.FirstBookingEnd = bookingHdrDto.FirstBookingEnd.ToLocalTime();
                            bookingHdr.ContactName = bookingHdrDto.ContactName;
                            bookingHdr.ContactEmail = bookingHdrDto.ContactEmail;
                            bookingHdr.ContactTel = bookingHdrDto.ContactTel;
                            bookingHdr.WarehouseCode = bookingHdrDto.WarehouseCode;
                            bookingHdr.SupCode = bookingHdrDto.SupCode;
                            bookingHdr.SupName = bookingHdrDto.SupName;
                            bookingHdr.TotalPo = bookingHdrDto.BookingDetails.Count();
                            bookingHdr.TotalQty = bookingHdrDto.BookingDetails.Sum(x => x.TotalQty);
                            bookingHdr.Postponed = bookingHdrDto.PostPoned;
                            bookingHdr.BackHaul = bookingHdrDto.BackHaul;
                            bookingHdr.Active = true;
                            //bookingHdr.Status = "NEW";
                            bookingHdr.Status = status;
                            bookingHdr.UserStamp = bookingKeyDto.UserName;
                            bookingHdr.FirstUserStamp = bookingKeyDto.UserName;
                            bookingHdr.CreateDate = DateTime.Now;
                            bookingHdr.Remark = bookingHdrDto.Remark;
                            bookingHdr.ModDate = DateTime.Now;
                            bookingHdr.MerchType = bookingHdrDto.MerchType;
                            bookingHdr.OriginalMerchType = bookingHdrDto.MerchType;
                            bookingHdr.RemarkDelay = bookingHdrDto.RemarkDelay;
                            bookingHdr.ApproveCondition = approvedCondition;

                            unitOfWork.BookingHeaderRepository.Add(bookingHdr);

                            bookingCreateList.Add(bookingHdr);

                            var bookingInterface = new BookingInterface();
                            bookingInterface.BookingId = bookingHdr.BookingId;
                            bookingInterface.IsInterface = false;
                            bookingInterface.DateTimeStamp = DateTime.Now;
                            bookingInterface.Status = status;
                            bookingInterface.UserStamp = userName;

                            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

                            unitOfWork.Save();

                            strResult.Add(bookingHdr.WarehouseCode + " : " + bookingId);

                            foreach (var bookingDtlDto in bookingHdrDto.BookingDetails)
                            {
                                bookingDtl = new BookingDetail();
                                bookingDtl = bookingDtlDto;
                                bookingDtl.InternalDetailKey = unitOfWork.PoListRepository.GetBookingDetailKey();
                                bookingDtl.InternalHeaderKey = bookingHdr.InternalHeaderKey;
                                bookingDtl.UserStamp = bookingKeyDto.UserName;
                                bookingDtl.Status = "NEW";
                                bookingDtl.CreateDate = DateTime.Now;
                                bookingDtl.ModDate = DateTime.Now;

                                unitOfWork.BookingDetailRepository.Add(bookingDtl);

                                if (bookingDtlDto.IsDelay.Value == true)
                                {
                                    unitOfWork.PoListRepository.UpdatePoPostponed(bookingDtlDto.PoNbr, bookingDtlDto.DelayReason);
                                    PoLog poLog = new PoLog();
                                    poLog.UserStamp = bookingKeyDto.UserName;
                                    poLog.DateTimeStamp = DateTime.Now;
                                    poLog.PoNbr = bookingDtl.PoNbr;
                                    poLog.Log = bookingDtl.DelayReason;
                                    unitOfWork.PoLogRepository.Add(poLog);

                                }

                                unitOfWork.Save();
                            }

                            foreach (var bookingTruckDto in bookingHdrDto.BookingTrucks)
                            {
                                bookingTruck = new BookingTruck();
                                bookingTruck = bookingTruckDto;
                                bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;
                                unitOfWork.BookingTruckRepository.Add(bookingTruck);
                            }
                        }
                        else
                        {
                            if((bookingHdr.Status == "APPROVED" || bookingHdr.Status == "INTRANSIT") && bookingHdr.ApproveCondition == "OVERCUTOFF")
                            {
                                status = "EDIT";
                            }
                            // update booking data   
                            if (bookingHdr.BookingStart != bookingHdrDto.BookingStart)
                            {
                                isSlottimeChange = true;

                                bookingChange.Add(bookingHdr.BookingId + "|" + bookingHdrDto.ContactEmail, bookingHdr.BookingStart.Value.ToString("dd/MM/yyyy HH:mm") + "|" + bookingHdrDto.BookingStart.ToLocalTime().ToString("dd/MM/yyyy HH:mm"));
                            }

                            if (string.IsNullOrEmpty(bookingHdr.ApproveCondition))
                            {
                                bookingHdr.ApproveCondition = approvedCondition;
                            }

                            var isDelay = false;
                            // update
                            bookingHdr.InternalDoorId = bookingHdrDto.InternalDoorId;
                            bookingHdr.BookingStart = bookingHdrDto.BookingStart.ToLocalTime();
                            bookingHdr.BookingEnd = bookingHdrDto.BookingEnd.ToLocalTime();
                            bookingHdr.ContactName = bookingHdrDto.ContactName;
                            bookingHdr.ContactEmail = bookingHdrDto.ContactEmail;
                            bookingHdr.ContactTel = bookingHdrDto.ContactTel;
                            bookingHdr.WarehouseCode = bookingHdrDto.WarehouseCode;
                            bookingHdr.SupCode = bookingHdrDto.SupCode;
                            bookingHdr.SupName = bookingHdrDto.SupName;
                            bookingHdr.TotalPo = bookingHdrDto.BookingDetails.Count();
                            bookingHdr.TotalQty = bookingHdrDto.BookingDetails.Sum(x => x.TotalQty);

                            bookingHdr.BackHaul = bookingHdrDto.BackHaul;
                            bookingHdr.Postponed = bookingHdrDto.PostPoned;
                            bookingHdr.MerchType = bookingHdrDto.MerchType;
                            bookingHdr.Active = true;

                            bookingHdr.RemarkDelay = bookingHdrDto.RemarkDelay;

                            bookingHdr.Status = status;
                           // bookingHdr.ApproveCondition = approvedCondition;
                            bookingHdr.UserStamp = bookingKeyDto.UserName;
                            bookingHdr.Remark = bookingHdrDto.Remark;
                            bookingHdr.ModDate = DateTime.Now;
                            
                            // remove po 
                            var bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHdr.InternalHeaderKey).ToList();
                            var listOldPo = new List<string>();
                            var listNewPo = new List<string>();
                            var listDelPo = new List<string>();
                            var listUpdatePo = new List<string>();

                            listOldPo = bookingDetails.Select(x=>x.PoNbr).ToList();
                            listNewPo = bookingHdrDto.BookingDetails.Select(x => x.PoNbr).ToList();
                            
                            listDelPo = listOldPo.Except(listNewPo).ToList();
                            listUpdatePo = listNewPo.Except(listDelPo).ToList();

                            foreach (var item in listDelPo)
                            {
                                unitOfWork.BookingDetailRepository.Remove(bookingDetails.FirstOrDefault(x => x.PoNbr == item));
                            }
                            
                            foreach (var bookingDtlDto in bookingHdrDto.BookingDetails)
                            {
                                // check duplicate po
                                bookingDtl = unitOfWork.BookingDetailRepository.GetBookingDetailById(bookingDtlDto.InternalDetailKey);

                                if (bookingDtl == null)
                                {
                                    bookingDtl = new BookingDetail();
                                    bookingDtl = bookingDtlDto;
                                    bookingDtl.InternalDetailKey = unitOfWork.PoListRepository.GetBookingDetailKey();
                                    bookingDtl.InternalHeaderKey = bookingHdr.InternalHeaderKey;
                                    bookingDtl.UserStamp = bookingKeyDto.UserName;
                                    bookingDtl.Status = "NEW";
                                    bookingDtl.CreateDate = DateTime.Now;
                                    bookingDtl.ModDate = DateTime.Now;

                                    unitOfWork.BookingDetailRepository.Add(bookingDtl);

                                    if (bookingDtlDto.IsDelay.Value == true)
                                    {
                                        unitOfWork.PoListRepository.UpdatePoPostponed(bookingDtlDto.PoNbr, bookingDtlDto.DelayReason);
                                        PoLog poLog = new PoLog();
                                        poLog.UserStamp = bookingKeyDto.UserName;
                                        poLog.DateTimeStamp = DateTime.Now;
                                        poLog.PoNbr = bookingDtl.PoNbr;
                                        poLog.Log = bookingDtl.DelayReason;
                                        unitOfWork.PoLogRepository.Add(poLog);
                                        isDelay = true;
                                    }

                                }
                                else
                                {
                                    bookingDtl.Postponed = bookingDtlDto.Postponed;
                                    bookingDtl.DelayReason = bookingDtlDto.DelayReason;
                                    bookingDtl.IsDelay = bookingDtlDto.IsDelay;
                                    bookingDtl.Remark = bookingDtlDto.Remark;

                                    unitOfWork.BookingDetailRepository.Update(bookingDtl);

                                    if (bookingDtlDto.IsDelay.Value == true)
                                    {
                                        unitOfWork.PoListRepository.UpdatePoPostponed(bookingDtlDto.PoNbr, bookingDtlDto.DelayReason);
                                        isDelay = true;
                                    }
                                }
                                unitOfWork.Save();
                            }

                            bookingHdr.IsDelay = isDelay;

                            var bookingInterface = new BookingInterface();
                            bookingInterface.BookingId = bookingHdr.BookingId;
                            bookingInterface.IsInterface = false;
                            bookingInterface.DateTimeStamp = DateTime.Now;
                            bookingInterface.Status = bookingHdr.Status;
                            bookingInterface.UserStamp = userName;

                            unitOfWork.BookingInterfaceRepository.Add(bookingInterface);

                            unitOfWork.BookingHeaderRepository.Update(bookingHdr);

                            if (bookingHdr.Status == "APPROVED")
                            {
                                bookingCreateList.Add(bookingHdr);
                            }

                            // remove truck
                            var bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingHdr.InternalHeaderKey).ToList();

                            if (bookingTrucks != null && bookingTrucks.Count > 0)
                            {
                                unitOfWork.BookingTruckRepository.Remove(bookingTrucks[0]);
                            }


                            foreach (var bookingTruckDto in bookingHdrDto.BookingTrucks)
                            {
                                bookingTruck = new BookingTruck();
                                bookingTruck = bookingTruckDto;
                                bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

                                unitOfWork.BookingTruckRepository.Add(bookingTruck);
                            }

                            unitOfWork.Save();
                        }

                        // remove pre-check in data
                        if (bookingHdrDto.InternalHeaderKey.HasValue)
                        {
                            var truckCheckIns = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInByHeaderId(bookingHdrDto.InternalHeaderKey.Value);

                            foreach (var truckCheckIn in truckCheckIns)
                            {
                                var truckCheckInDtls = unitOfWork.BookingTruckCheckInDetailRepository.GetByCheckInId(Convert.ToInt32(truckCheckIn.InternalTruckCheckInId));
                                unitOfWork.BookingTruckCheckInDetailRepository.Remove(truckCheckIn.InternalTruckCheckInId);
                                unitOfWork.BookingTruckCheckInRepository.Remove(truckCheckIn);
                            }

                        }
                    }

                    unitOfWork.Save();

                    // get bookingHeader by bookingKey
                    var bkHdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingKeyDto.InternalKeyId.Value).ToList();
                    if (bkHdr.Count == 0)
                    {
                        // remove booking key
                        unitOfWork.BookingKeyRepository.Remove(bookingKey);
                        unitOfWork.Save();
                    }

                    try
                    {
                        // send mail for booking approved.
                        mailSend(bookingCreateList);


                        if (user.UserType != "SUP" && user.UserType != "BH" && user.UserType != "SUPTRAN")
                        {
                            mailChangeTimeSlot(bookingChange);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                // rollback transaction                
                throw new Exception(ex.Message);
            }
            return strResult;
        }

        public async Task<BookingKeyDto> GetBookingKeyByBookingKeyId(int bookingKeyId)
        {
            BookingKeyDto bookingKeyDto = new BookingKeyDto();
            BookingKey bookingKey = new BookingKey();
            BookingDetail bookingDetail = new BookingDetail();
            List<BookingHeader> bookingHeaders = new List<BookingHeader>();
            List<BookingTruck> bookingTrucks = new List<BookingTruck>();
            List<BookingDetail> bookingDetails = new List<BookingDetail>();
            List<TruckMaster> truckMasters = new List<TruckMaster>();
            List<BookingTruckCheckInDto> bookingCheckIns = new List<BookingTruckCheckInDto>();

            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            bookingKey = unitOfWork.BookingKeyRepository.GetBookingKeyById(bookingKeyId);            

            bookingHeaders = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingKeyId).ToList();

            bookingKeyDto.InternalKeyId= bookingKeyId;
            bookingKeyDto.BookingHeaders = new List<BookingHeaderDto>();

            foreach (var hdr in bookingHeaders.OrderBy(x=>x.WarehouseCode))
            {
                var bookingHdr = new BookingHeaderDto();

                bookingHdr.BookingId = hdr.BookingId;
                bookingHdr.InternalHeaderKey= hdr.InternalHeaderKey;
                bookingHdr.SupCode = hdr.SupCode;
                bookingHdr.ContactName = hdr.ContactName;
                bookingHdr.ContactEmail = hdr.ContactEmail;
                bookingHdr.ContactTel = hdr.ContactTel;
                bookingHdr.BookingStart = hdr.BookingStart.Value;
                bookingHdr.BookingEnd = hdr.BookingEnd.Value;
                bookingHdr.WarehouseCode = hdr.WarehouseCode;
                bookingHdr.SupName = hdr.SupName;
                bookingHdr.InternalSupGroupId = hdr.InternalSupGroupId.Value;
                bookingHdr.Status = hdr.Status;
                bookingHdr.InternalDoorId = hdr.InternalDoorId == null ? 0 : hdr.InternalDoorId.Value;
                bookingHdr.DockDoor = hdr.InternalDoorId == null || hdr.InternalDoorId == 0 ? "-" : unitOfWork.DoorRepository.GetDoorById(hdr.InternalDoorId.Value).DoorName;
                bookingHdr.MerchType = hdr.MerchType;
                bookingHdr.CompanyCode = hdr.CompanyCode;
                bookingHdr.BackHaul = hdr.BackHaul.Value;
                bookingHdr.PostPoned = hdr.Postponed.Value;
                bookingHdr.RemarkDelay = hdr.RemarkDelay;
                bookingHdr.PostPoned = hdr.Postponed == null ? false : hdr.Postponed.Value;
                bookingHdr.FirstBookingStart = hdr.FirstBookginStart.Value;
                bookingHdr.FirstBookingEnd = hdr.FirstBookingEnd.Value;
                bookingHdr.FirstUserStamp = hdr.FirstUserStamp;
                bookingHdr.ModDate = hdr.ModDate;
                bookingHdr.UserStamp = hdr.UserStamp;
                bookingHdr.Remark = hdr.Remark;
                    
                // get booking detail of this header
                bookingDetails = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(hdr.InternalHeaderKey).ToList();
                bookingHdr.BookingDetails = bookingDetails;

                // get booking truck of this header
                bookingTrucks= unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(hdr.InternalHeaderKey).ToList();
                bookingHdr.BookingTrucks= bookingTrucks;

                
                bookingKeyDto.BookingDate = bookingKey.BookingDate.Value;
                

                bookingKeyDto.BookingHeaders.Add(bookingHdr);
            }

            return bookingKeyDto;
        }

        public bool CheckCapBooking(ref BookingKeyDto bookingKeyDto)
        {
            WarehouseCapacity warehouseCapacity = new WarehouseCapacity();
            Warehouse warehouse = new Warehouse();
            List<Operation> operation = unitOfWork.OperationRepository.GetOperations().ToList();

            var supgroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingKeyDto.BookingHeaders[0].InternalSupGroupId);

            if(supgroup.IsVip == "Y")
            {
                return true;
            }
            foreach (var bookingHdrDto in bookingKeyDto.BookingHeaders)
            {                
                // generate booking header id
                warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(bookingHdrDto.WarehouseCode);

                var whseBookingCap = wcs.GetOperationCapacityByBookingDate(bookingKeyDto.BookingDate.ToLocalTime(), warehouse.WarehouseMain, bookingHdrDto.MerchType).Result;

                //var checkCaps = whseBookingCap.Where(x => x.BookingDateTime >= bookingHdrDto.BookingStart.ToLocalTime() && x.BookingDateTime < bookingHdrDto.BookingEnd.ToLocalTime());
                var checkCaps = whseBookingCap.Where(x => x.BookingDateTime.Hour == bookingHdrDto.BookingStart.ToLocalTime().Hour && x.BookingDateTime.Minute == bookingHdrDto.BookingStart.ToLocalTime().Minute);


                decimal? totalCurrentWeight = 0;

                IEnumerable<BookingDetail> currentBooking = null;

                if (bookingHdrDto.InternalHeaderKey.HasValue)
                {
                    currentBooking = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bookingHdrDto.InternalHeaderKey.Value);
                }

                if(currentBooking != null)
                {
                    totalCurrentWeight = currentBooking.Sum(x => x.Weight);
                }
                else
                {
                    totalCurrentWeight = 0;
                }

                var totalWeight = bookingHdrDto.BookingDetails.Sum(x => x.Weight);

                

                foreach (var cap in checkCaps)
                {
                    // full pl = booking weight
                    // cube full = max capacity
                 
                    if (cap.FULL_PL - totalCurrentWeight + totalWeight > cap.CUBE_FULL && cap.CUBE_FULL > 0)
                    {
                        if(operation.FirstOrDefault(x=>x.WarehouseCode == bookingHdrDto.WarehouseCode && x.OperationName == bookingHdrDto.MerchType).Overcap == false)
                        {
                            bookingHdrDto.Status = "FAIL";
                        }
                        else
                        {
                            if (bookingHdrDto.Status != "OVERCUTOFF") {
                                bookingHdrDto.Status = "OVERCAP";
                            }
                        }

                        //return false;
                    }
                    
                }
            }
            return true;
        }


        private void mailSend(List<BookingHeader> bookingList)
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


            foreach (BookingHeader bookingHdr in bookingList)
            {
                if(bookingHdr.Status == "APPROVED")
                {
                  //  var supGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingHdr.InternalSupGroupId.Value);
                    
                    if (bookingHdr.ContactEmail.Contains("@"))
                    {
                        Helper.Email.SendEmail(bookingHdr.ContactEmail, string.Format(htmlContent,bookingHdr.BookingId), "Makro : IMS Booking Approvaled");
                    }

                }
            }
        }

        private void mailChangeTimeSlot(Dictionary<string,string> bookingList)
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
                                    ทีม Booking ได้ทำการเลื่อนรอบการจัดส่งของท่าน สามารถตรวจสอบข้อมูลในระบบ <strong>IMS</strong> ได้เลยค่ะ
                                </p>
                                <p style='color: #333;text-indent: 2em;'>
                                    หากติดปัญหาไม่สามารถจัดส่งได้ตามวันและเวลาดังกล่าว กรุณาติดต่อ Booking คะ                                     
                                </p>
                                <p style='color: #333; font-weight: bold;'>
                                    หมายเลข Booking ID : <span style='color: #4caf50;'>{0}</span> <br>
                                    เดิม : {1} <br>
                                    ใหม่ : {2}
                                </p>
                                <p style='color: #333; font-weight: bold;'>
                                    *หมายเหตุ : เนื่องจากทาง DC ไม่สามารถรับสินค้าช่วงเวลาดังกล่าวให้ทางซัพพลายเออร์ได้ ทำให้การรอลงสินค้าอาจใช้เวลานานมากกว่าปกติ
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


            foreach (var bookingHdr in bookingList)
            {
                var mail = bookingHdr.Key.Split("|")[1];
                var bookingId = bookingHdr.Key.Split("|")[0];
                var oldTime = bookingHdr.Value.Split("|")[0];
                var newTime = bookingHdr.Value.Split("|")[1];
                    //  var supGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingHdr.InternalSupGroupId.Value);
                    if (mail.Contains("@"))
                    {
                        Helper.Email.SendEmail(mail, string.Format(htmlContent, bookingId,oldTime,newTime), "Makro : IMS Booking เลื่อนรอบจัดส่ง");
                    }                
            }
        }

    }
}
