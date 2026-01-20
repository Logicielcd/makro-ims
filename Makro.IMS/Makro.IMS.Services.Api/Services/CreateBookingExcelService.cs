using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;

namespace Makro.IMS.Services.Api.Services
{
    public class CreateBookingExcelService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private WarehouseCapacityService warehouseCapacityService;
        private BookingKeyService bookingKeyService;

        public CreateBookingExcelService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            warehouseCapacityService = new WarehouseCapacityService();
            bookingKeyService = new BookingKeyService();
        }

        public async Task<CreateBookingExcelDto> Import(CreateBookingExcelDto createBookingExcelDto, string userName)
        {
            BookingKeyDto bookingKeyDto = new BookingKeyDto();
            bool canImportfile = true;

            unitOfWork.BeginTransaction(System.Data.IsolationLevel.Serializable);

            // get supplier information
            Supplier supplier = unitOfWork.SupplierRepository.GetSupplierBySupCode(createBookingExcelDto.SupplierCode);
            SupplierGroup supplierGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(createBookingExcelDto.InternalSupGroupId);
            
            List<PoList> poLists = new List<PoList>();
            List<BookingHeaderDto> bookingHeaderDtos = new List<BookingHeaderDto>();
            List<Warehouse> warehouses = unitOfWork.WarehouseRepository.GetWarehouses().ToList();

            List<Operation> operations = unitOfWork.OperationRepository.GetOperations().ToList();

            bookingKeyDto.BookingHeaders = new List<BookingHeaderDto>();

            createBookingExcelDto.BookingDate = createBookingExcelDto.BookingDate.ToLocalTime().Date;
            createBookingExcelDto.BookingHeaders = new List<BookingHeader>();

            var bookingDate = createBookingExcelDto.BookingDate;

            // get po information
            // check po
            foreach (var po in createBookingExcelDto.CreateBookingExcelWarehouses)
            {
                po.TimeSlot = po.TimeSlot.Value.ToLocalTime();

                var poInfo = unitOfWork.PoListRepository.GetPoByPoNo(po.PoNo, supplier.SupCode, "");
                if(poInfo == null)
                {
                    canImportfile = false;
                    var poCheck = unitOfWork.PoListRepository.GetPoByPo(po.PoNo);

                    if (poCheck == null)
                    {                     
                        po.ImportResult = "PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(poCheck.First().Booking_Id))
                        {
                            if (poCheck.FirstOrDefault().Booking_Id == "0")
                            {
                                po.ImportResult = "PO incorrect supplier group, Please contact data entry" + "\r\n" + "หมายเลขใบสั่งซื้อนี้มีข้อมูล supplier group ไม่ถูกต้อง, กรุณาติดต่อ Data entry";
                            }
                            else
                            {
                                po.ImportResult = "PO already book, Please contact data entry" + "\r\n" + "หมายเลขใบสั่งซื้อนี้มีการนัดหมายแล้ว, กรุณาติดต่อ Data entry";
                            }
                        }
                    }
                }
                else
                {
                    poInfo.Remark = po.Remark;
                    if (poInfo.Warehouse_Code != po.WarehouseCode)
                    {
                        // check po main
                        var warehouse = warehouses.FirstOrDefault(x => x.WarehouseCode == poInfo.Warehouse_Code);

                        if (warehouse != null && warehouse.WarehouseMain == po.WarehouseCode)
                        {
                            po.WarehouseCode = poInfo.Warehouse_Code;
                        }
                        else
                        {
                            canImportfile = false;
                            po.ImportResult = "PO incorrect warehouse, Please contact data entry" + "\r\n" + "หมายเลขใบสั่งซื้อนี้ส่งผิดคลังสินค้า, กรุณาติดต่อ Data entry";
                        }
                    }
                    else
                    {
                        if (poInfo.Booking_Id != "0")
                        {
                            canImportfile = false;
                            po.ImportResult = "PO already book, Please contact data entry" + "\r\n" + "หมายเลขใบสั่งซื้อนี้มีการนัดหมายแล้ว, กรุณาติดต่อ Data entry";
                        }
                        if (bookingDate.Date >= poInfo.Expire_Date.Value.Date)
                        {
                            canImportfile = false;
                            po.ImportResult = "PO already expired, PO expire date is " + poInfo.Expire_Date.Value.ToString("dd/MM/yyyy") + "\r\n" + "หมายเลขใบสั่งซื้อนี้หมดอายุแล้ว, กรุณาติดต่อ Data entry";
                        }
                    }
                    po.poInfo = poInfo;
                    poLists.Add(poInfo);
                }
            }
            
            if (canImportfile)
            {
                List<TruckMaster> truckMasters = unitOfWork.TruckMasterRepository.GetTrucks().ToList();
              //  List<Warehouse> warehouses = unitOfWork.WarehouseRepository.GetWarehouses().ToList();
                bookingKeyDto = this.CreateBookingKey(createBookingExcelDto.BookingDate,userName);
                bookingHeaderDtos = this.CreateBookingHeader(createBookingExcelDto, warehouses, truckMasters, supplier, supplierGroup);

                List<BookingImportResultDto> bhResult = new List<BookingImportResultDto>();

                // check available time slot
                foreach (var bookingHdr in bookingHeaderDtos)
                {
                    var bookingResult = this.AssignTimeslot(bookingDate, bookingHdr,ref bhResult,operations);

                    if (bookingResult.Status == "FAIL")
                    {
                        //canImportfile = false;
                        foreach (var po in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => bookingResult.BookingDetails.Any(a => a.PoNbr == x.PoNo)))
                        {
                            po.ImportResult = bookingResult.ContactName;
                        }
                    }
                    else
                    {                        
                        if(bookingResult.DockDoor == "1")
                        {
                            foreach(var po in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x=> bookingResult.BookingDetails.Any(a=>a.PoNbr == x.PoNo)))
                            {
                                po.SystemOverride = true;                                
                            }
                        }
                        foreach (var po in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => bookingResult.BookingDetails.Any(a => a.PoNbr == x.PoNo)))
                        {                            
                            po.OperationType = bookingResult.MerchType;
                        }
                        if(bookingResult.Status == "OVERCAP")
                        {
                            bookingHdr.Status = "OVERCAP";
                        }
                        if(bookingResult.Status == "OVERCUTOFF")
                        {
                           bookingHdr.Status = "OVERCUTOFF";
                        }
                        bookingHdr.BookingStart = bookingResult.BookingStart;
                        bookingHdr.BookingEnd = bookingResult.BookingEnd;
                        if (bookingHdr.Status != "FAIL")
                        {
                            bookingKeyDto.BookingHeaders.Add(bookingHdr);
                        }
                    }
                }

                if (canImportfile)
                {
                    var saveResult = await bookingKeyService.SaveBooking(bookingKeyDto,"IMPORT");

                    unitOfWork.Commit();

                    var bookingIdList = new List<string>();

                    foreach (var result in saveResult)
                    {
                        bookingIdList.Add(result.Split(':')[1].ToString().Trim());
                    }

                    foreach (var item in bookingIdList.Distinct())
                    {
                        var bh = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(item);
                        createBookingExcelDto.BookingHeaders.Add(bh);

                        var details = unitOfWork.BookingDetailRepository.GetBookingDetailsByHeaderId(bh.InternalHeaderKey);

                        foreach (var detail in details)
                        {
                            var cbs = createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.PoNo == detail.PoNbr);
                            foreach (var cb in cbs)
                            {
                                cb.ImportResult = "Booking ID : " + item + "\r\n" +
                                    "Booking Slot : " + bh.BookingStart.Value.ToString("dd/MM/yyyy HH:mm");
                            }
                        }
                    }

                    //foreach (var item in createBookingExcelDto.CreateBookingExcelWarehouses)
                    //{                        
                        //var bookingId = unitOfWork.PoListRepository.GetPoByPo(item.PoNo).First();

                        //if (bookingId != null)
                        //{                            
                        //    item.ImportResult = "Booking ID : " + saveResult[0].Split(':')[1].ToString() + "\r\n" +
                        //        "Booking Slot : " + bookingKeyDto.BookingHeaders.FirstOrDefault(x => x.BookingDetails.Any(y => y.PoNbr == item.PoNo)).BookingStart.ToString("dd/MM/yyyy HH:mm");
                        //    bookingIdList.Add(saveResult[0].Split(':')[1].ToString());// (bookingId.Remark);
                        //}                 
                   // }

                }
                else
                {
                    unitOfWork.Rollback();
                }
            }
            else
            {
                unitOfWork.Rollback();
                return createBookingExcelDto;
            }

            
            return createBookingExcelDto;
        }

        private BookingKeyDto CreateBookingKey(DateTime bookingDate,string userName)
        {
            BookingKeyDto bk = new BookingKeyDto();
            bk.BookingHeaders = new List<BookingHeaderDto>();
            bk.BookingDate = bookingDate;
            bk.UserName = userName;
            return bk;
        }

        private List<BookingHeaderDto> CreateBookingHeader(CreateBookingExcelDto createBookingExcelDto,List<Warehouse> warehouses,List<TruckMaster> truckMasters,Supplier supplier,SupplierGroup supplierGroup)
        {
            DateTime bookingDate = createBookingExcelDto.BookingDate.ToLocalTime().Date;
            List<TruckCap> truckCaps = unitOfWork.TruckCapRepository.GetTruckCaps().ToList();
            Warehouse? whse = new Warehouse();
            List<BookingHeaderDto> bookingHeaders = new List<BookingHeaderDto>();
            List<EstTime> estTimes = unitOfWork.EstTimeRepository.GetEstTimes().ToList();
            List<TruckRule> truckRules = unitOfWork.TruckRuleRepository.GetTruckRules().ToList();
            
            List<string> groupBooking = createBookingExcelDto.CreateBookingExcelWarehouses.Select(x => x.BookingGroup).Distinct().ToList();

            foreach (var group in groupBooking)
            {
                var bookingGroup = createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.BookingGroup == group).ToList();
                var poWhseCom = bookingGroup.Select(x => new { x.poInfo.Warehouse_Code, x.poInfo.Company_Code }).Distinct().ToList();                
                if (poWhseCom.Count > 1)
                {
                    // multiple warehouse company
                }
                foreach (var item in poWhseCom) 
                {
                    var poImport = new List<string>();
                    var bookingDtl = bookingGroup.Where(x => x.poInfo.Warehouse_Code == item.Warehouse_Code && x.poInfo.Company_Code == item.Company_Code).ToList();                   

                    whse = warehouses.FirstOrDefault(x => x.WarehouseWms == item.Warehouse_Code && x.CompanyCode == item.Company_Code);

                    BookingHeaderDto bookingHdr = new BookingHeaderDto();
                    bookingHdr.BookingDetails = new List<BookingDetail>();
                    bookingHdr.BookingTrucks = new List<BookingTruck>();                    
                    bookingHdr.BackHaul = false;
                    bookingHdr.PostPoned = false;
                    bookingHdr.InternalSupGroupId = createBookingExcelDto.InternalSupGroupId;
                    bookingHdr.SupCode = supplier.SupCode;
                    bookingHdr.SupName = supplier.SupName;
                    bookingHdr.WarehouseCode = whse.WarehouseCode;
                    bookingHdr.CompanyCode = whse.CompanyCode;
                    bookingHdr.ContactName = createBookingExcelDto.ContactName;
                    bookingHdr.ContactTel = createBookingExcelDto.ContactPhone;
                    bookingHdr.ContactEmail = createBookingExcelDto.ContactEmail;
                    bookingHdr.RemarkDelay = "";

                    foreach (var po in bookingDtl)
                    {
                        if (!poImport.Contains(po.PoNo))
                        {
                            poImport.Add(po.PoNo);
                            if (whse == null)
                            {
                                po.ImportResult = "Warehouse not found";
                            }
                            else
                            {
                                BookingDetail bookingDetail = new BookingDetail();
                                bookingDetail.PoNbr = po.poInfo.Po_Nbr;
                                bookingDetail.PlanRec = po.poInfo.Plan_Receive_Date;
                                bookingDetail.ExpireDate = po.poInfo.Expire_Date;
                                bookingDetail.MerchType = po.poInfo.Merch_Type;
                                bookingDetail.TotalQty = po.poInfo.Total_Qty;
                                bookingDetail.FullPl = po.poInfo.Full;
                                bookingDetail.FullCs = po.poInfo.Full_Cs == null ? 0 : po.poInfo.Full_Cs;
                                bookingDetail.HalfPl = (long)(po.poInfo.Half == null ? 0 : po.poInfo.Half);
                                bookingDetail.HalfCs = po.poInfo.Half_Cs == null ? 0 : po.poInfo.Half_Cs;
                                bookingDetail.Con = po.poInfo.Con;
                                bookingDetail.Non = po.poInfo.Non;
                                bookingDetail.Weight = po.poInfo.Weight;
                                bookingDetail.CubeFull = po.poInfo.Cube_Full;
                                bookingDetail.CubeCon = po.poInfo.Cube_Con;
                                bookingDetail.CubeNon = po.poInfo.Cube_Non;
                                bookingDetail.IsDelay = po.poInfo.Plan_Receive_Date >= createBookingExcelDto.BookingDate ? false : true;
                                bookingDetail.Postponed = po.poInfo.PostPoned;
                                bookingDetail.Remark = po.poInfo.Remark;
                                if (po.DeliveryType.ToLower() == "backhaul")
                                {
                                    bookingHdr.BackHaul = true;
                                }
                                else
                                {
                                    bookingHdr.BackHaul = false;
                                }

                                bookingHdr.BookingDetails.Add(bookingDetail);
                            }
                        }
                        
                    }

                    var trucks = bookingDtl.GroupBy(x => x.TruckType)
                                .Select(g => new
                                {
                                    truckType = g.Key,
                                    totalTruck = g.Select(p=>p.TruckNo).Distinct().Count()
                                });

                    foreach (var truck in trucks)
                    {
                        var tm = truckMasters.FirstOrDefault(x => x.TruckCode == truck.truckType.ToString());
                        
                        BookingTruck bt = new BookingTruck();
                        bt.InternalTruckId = tm.InternalTruckId;
                        bt.TotalTruck = truck.totalTruck;
                        bookingHdr.BookingTrucks.Add(bt);
                    }

                    var largestTruck = bookingHdr.BookingTrucks.GroupBy(g=>g.InternalHeaderKey)
                        .Select(group => new
                        {
                            internalHeader = group.Key,
                            largestTruck = group
                                .Join(truckMasters,
                                    g => g.InternalTruckId,
                                    t => t.InternalTruckId,
                                    (g,t) => t)
                                .OrderByDescending(t=>t.Sequence)
                                .FirstOrDefault()                                
                        });

                    var largestTruckId = 0;

                    foreach (var result in largestTruck)
                    {
                        largestTruckId = result.largestTruck.InternalTruckId;
                    }

                    if(whse.CapUom == "CS") {
                        var truckCap = truckCaps.FirstOrDefault(x => x.InternalTruckId == largestTruckId && x.WarehouseCode == whse.WarehouseCode);
                        var truckMaster = truckMasters.FirstOrDefault(x => x.InternalTruckId == largestTruckId);

                        var qtyType = "";
                                                
                        // check merch type
                        var totalFull = bookingHdr.BookingDetails.Sum(x => x.FullPl);
                        var totalHalf = bookingHdr.BookingDetails.Sum(x => x.HalfPl);
                        var totalCon = bookingHdr.BookingDetails.Sum(x => x.Con);
                        var totalNon = bookingHdr.BookingDetails.Sum(x => x.Non);

                        totalFull += totalHalf / 2;

                        if (truckCap is not null && totalFull >= truckCap.FullPl)
                        {
                            qtyType = "FULL";
                        }
                        else if(totalCon > 0 || totalNon > 0 || totalFull > 0 || totalHalf > 0)
                        {
                            qtyType = totalCon > totalNon ? "CON" : "NON";                            
                        }
                        else
                        {
                            qtyType = "";
                        }

                        var truckRule = truckRules.FirstOrDefault(x => x.InternalTruckId.Split('|').Contains(largestTruckId.ToString()) 
                        && x.Warehouse == whse.WarehouseCode
                        && x.QtyType.Split('|').Contains(qtyType));

                        if (truckRule != null)
                        {
                            bookingHdr.MerchType = truckRule.OperationType;
                        }
                        else {
                            var sumMerchType = bookingHdr.BookingDetails.GroupBy(x => x.MerchType).Select(g => new
                            {
                                merchType = g.Key,
                                totalQty = g.Sum(x => x.TotalQty)
                            }).OrderByDescending(x => x.totalQty).FirstOrDefault();

                            bookingHdr.MerchType = sumMerchType.merchType;
                            bookingHdr.TotalTruck = bookingHdr.BookingTrucks.Sum(x => x.TotalTruck).ToString();
                        }                        
                    }
                    else
                    {
                        var sumMerchType = bookingHdr.BookingDetails.GroupBy(x => x.MerchType).Select(g => new
                        {
                            merchType = g.Key,
                            totalQty = g.Sum(x=>x.TotalQty)
                        }).OrderByDescending(x=>x.totalQty).FirstOrDefault();

                        bookingHdr.MerchType = sumMerchType.merchType;
                        bookingHdr.TotalTruck = bookingHdr.BookingTrucks.Sum(x => x.TotalTruck).ToString();
                    }

                    // booking header set booking start booking end

                    bookingHdr.BookingStart = bookingDate;

                    if(whse.FirstTimeOfDay > whse.EndTimeOfDay)
                    {
                        if(bookingDtl.First().TimeSlot.Value.Hour > whse.FirstTimeOfDay)
                        {
                            bookingHdr.BookingStart = bookingHdr.BookingStart.AddDays(-1);
                            bookingHdr.BookingStart = bookingHdr.BookingStart.AddHours(bookingDtl.First().TimeSlot.Value.Hour);
                            bookingHdr.BookingStart = bookingHdr.BookingStart.AddMinutes(bookingDtl.First().TimeSlot.Value.Minute);
                        }
                        else
                        {
                            bookingHdr.BookingStart = bookingHdr.BookingStart.AddHours(bookingDtl.First().TimeSlot.Value.Hour);
                            bookingHdr.BookingStart = bookingHdr.BookingStart.AddMinutes(bookingDtl.First().TimeSlot.Value.Minute);
                        }
                    }
                    else
                    {
                        bookingHdr.BookingStart = bookingHdr.BookingStart.AddHours(bookingDtl.First().TimeSlot.Value.Hour);
                        bookingHdr.BookingStart = bookingHdr.BookingStart.AddMinutes(bookingDtl.First().TimeSlot.Value.Minute);
                    }

                    var estimate = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supplierGroup.InternalSupGroupId && x.InternalTruckId == largestTruckId && x.OperationType.Contains(bookingHdr.MerchType));

                    bookingHdr.BookingEnd = bookingHdr.BookingStart;
                    bookingHdr.BookingEnd = bookingHdr.BookingEnd.AddHours(estimate.HourEst.Value);
                    bookingHdr.BookingEnd = bookingHdr.BookingEnd.AddMinutes(estimate.MinEst.Value);
                    bookingHdr.FirstBookingStart = bookingHdr.BookingStart;
                    bookingHdr.FirstBookingEnd = bookingHdr.BookingEnd;

                    bookingHeaders.Add(bookingHdr);
                }                
            }
            
            return bookingHeaders;
        }

        private BookingHeaderDto AssignTimeslot(DateTime bookingDate, BookingHeaderDto bookingHeader,ref List<BookingImportResultDto> bhResult,List<Operation> operations)
        {
            var totalQty = bookingHeader.BookingDetails.Sum(x => x.Weight);
            var totalTruck = bookingHeader.BookingTrucks.Sum(x => x.TotalTruck);

            var minEstimate = (bookingHeader.BookingEnd - bookingHeader.BookingStart).TotalMinutes;

            var whse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(bookingHeader.WarehouseCode);

            var minBookingDate = DateTime.Now;

            var nextShift = DateTime.Now;

            var currentTime = DateTime.Now;

            var overCutoff = false;

            var operation = operations.FirstOrDefault(x => x.WarehouseCode == bookingHeader.WarehouseCode && x.OperationName == bookingHeader.MerchType);

            if(whse.AdvanceBookingDay > -1)
            {
                nextShift = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, whse.FirstTimeOfDay.Value, 0, 0);
                if(whse.FirstTimeOfDay < whse.EndTimeOfDay)
                {
                    nextShift = nextShift.AddDays(1);
                }

                currentTime.AddHours(whse.AdvanceBookingDay.Value);

                if(currentTime > nextShift)
                {
                    overCutoff = true;
                }

            }
            
           // var minDate = DateTime.Now;

            if (whse.AdvanceBookingDay > -1)
            {
                //minBookingDate = DateTime.Now.AddDays(Convert.ToDouble(whse.AdvanceBookingDay));
                   
                //minBookingDate = whse.FirstTimeOfDay > whse.EndTimeOfDay ? minBookingDate.AddDays(1) : minBookingDate;
                //minBookingDate = DateTime.Now.Hour >= whse.FirstTimeOfDay ? minBookingDate.AddDays(1) : minBookingDate;

                //minBookingDate = minBookingDate.Hour >= whse.EndTimeOfDay ? minBookingDate.AddDays(1) : minBookingDate;

                minBookingDate = whse.FirstTimeOfDay > whse.EndTimeOfDay ? nextShift.AddDays(1) : nextShift;
            }
            else
            {
                minBookingDate = DateTime.Now.AddMinutes(Convert.ToDouble(whse.AdvanceBookingPeriod));                
               // minDate = DateTime.Now.AddHours(Convert.ToDouble(whse.AdvanceBookingPeriod));
            }

            if (bookingDate.Date < minBookingDate.Date)
            {
                bookingHeader.Status = "FAIL";
                bookingHeader.ContactName = "วันที่สามารถทำการ booking ได้คือ " + minBookingDate.ToString("dd/MM/yyyy") + " เป็นต้นไป";
                return bookingHeader;
            }

            var slotCaps = unitOfWork.SlotCapacityRepository.GetSlotCapacities(bookingDate, whse.WarehouseMain, bookingHeader.MerchType, 0).ToList();

            var startTime = whse.FirstTimeOfDay > whse.EndTimeOfDay ? bookingDate.Date.AddDays(-1).AddHours(Convert.ToDouble(whse.FirstTimeOfDay))
                : bookingDate.Date.AddHours(Convert.ToDouble(whse.FirstTimeOfDay));

            var endTime = bookingDate.Date.AddHours(Convert.ToDouble(whse.EndTimeOfDay));

            slotCaps = slotCaps.Where(x => DateTime.ParseExact(x.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) >= startTime
            && DateTime.ParseExact(x.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) <= endTime).ToList();

            var availableTime = unitOfWork.OperationTimeRepository.GetOperationTimes()
                .Where(x=>x.WarehouseCode == bookingHeader.WarehouseCode && x.OperationType == bookingHeader.MerchType).ToList();
            
            var fixSlots = unitOfWork.OperationFixSlotRepository.GetOperationFixSlotByWhseAndOpType(bookingHeader.WarehouseCode, bookingHeader.MerchType)
                .Where(x=>x.SupGroupId == bookingHeader.InternalSupGroupId).ToList();
            
            var supGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(bookingHeader.InternalSupGroupId);

            var whseCutoff = supGroup.WarehouseCutoff.Split("|").ToList();

            var supCutoff = whseCutoff.Contains(bookingHeader.WarehouseCode) ? true : false;

            var isVip = false;

            if (supGroup.IsVip == "N")
            {
                slotCaps = slotCaps.Where(x => x.Cap > 0 && x.CapTruck > 0).ToList();
            }
            else
            {
                if (!supGroup.Warehouses.Split("|").Contains(bookingHeader.WarehouseCode))
                {
                    slotCaps = slotCaps.Where(x => x.Cap > 0 && x.CapTruck > 0).ToList();
                    isVip = true;
                }
            }
            
            if(fixSlots.Count > 0)
            {
                slotCaps = slotCaps.Where(item1 => fixSlots.Any(item2 => item2.StartTime.ToString("hh:mm") == item1.TimeSlot)).ToList();
            }

            // add cap for booking already book into slotCaps

            if (!isVip)
            {
                foreach (var cap in slotCaps)
                {
                    if (bhResult.Count(x => x.TimeSlot == Convert.ToDateTime(cap.TimeSlot)) > 0)
                    {
                        cap.TotalWeight += bhResult.Where(x => x.TimeSlot == Convert.ToDateTime(cap.TimeSlot)).Sum(x => x.TotalQty);
                        cap.TotalTruck += bhResult.Where(x => x.TimeSlot == Convert.ToDateTime(cap.TimeSlot)).Sum(x => x.TotalTruck);
                    }
                }
            }
            // ลบข้อมูลใน List 1 ที่ไม่มีใน List 2 ตาม Date
            //slotCaps = slotCaps.Where(item1 => availableTime.Any(item2 => item2.StartTime == DateTime.ParseExact(item1.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture))).ToList();

            //slotCaps = slotCaps.Where(item1 => availableTime.Any(item2 => item2.StartTime.ToString("hh:mm") == item1.TimeSlot)).ToList();

            var slot = slotCaps.FirstOrDefault(x => x.TimeSlot == bookingHeader.BookingStart.ToString("yyyy-MM-dd HH:mm"));
           
            if( slot == null || (slot.TotalWeight + totalQty > slot.Cap) || (slot.TotalTruck + totalTruck > slot.CapTruck))
            {
                // system search new available slot
                var newSlots = slotCaps.Where(x => x.Cap >= x.TotalWeight + totalQty && x.CapTruck >= x.TotalTruck + totalTruck).ToList();
                var nearMoreSlot = newSlots.Where(x => DateTime.ParseExact(x.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) > bookingHeader.BookingStart)
                    .OrderBy(x=>x.TimeSlot).FirstOrDefault();
                var nearLessSlot = newSlots.Where(x => DateTime.ParseExact(x.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) < bookingHeader.BookingStart)
                    .OrderByDescending(x => x.TimeSlot).FirstOrDefault();

                var nearMoreDate = nearMoreSlot != null ? DateTime.ParseExact(nearMoreSlot.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                var nearLessDate = nearLessSlot != null ? DateTime.ParseExact(nearLessSlot.TimeSlot, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;

                nearMoreDate = DateTime.SpecifyKind(nearMoreDate, DateTimeKind.Local);
                nearLessDate = DateTime.SpecifyKind(nearLessDate, DateTimeKind.Local);


                if (nearMoreDate == DateTime.MinValue && nearLessDate == DateTime.MinValue)
                {
                    // check operation allow to over cap or not.
                    if(operations.FirstOrDefault(x=>x.WarehouseCode == bookingHeader.WarehouseCode && x.OperationName == bookingHeader.MerchType).Overcap == true)
                    {
                        bookingHeader.Status = "OVERCAP";
                    }
                    else
                    {
                        bookingHeader.Status = "FAIL";
                        bookingHeader.ContactName = "Not found available time slot";
                    }                    
                }
                else
                {
                    if (Math.Abs((bookingHeader.BookingStart - nearLessDate).TotalMinutes) > Math.Abs((nearMoreDate - bookingHeader.BookingStart).TotalMinutes))
                    {
                        bookingHeader.BookingStart = nearMoreDate;
                    }
                    else if (nearLessDate > DateTime.MinValue)
                    {
                        {
                            bookingHeader.BookingStart = nearLessDate;
                        }
                    }
                }

                if(overCutoff && operation.Overcutoff.Value && supCutoff)
                {
                    bookingHeader.Status = "OVERCUTOFF";
                }

                bookingHeader.BookingEnd = bookingHeader.BookingStart.AddMinutes(minEstimate);
                bookingHeader.DockDoor = "1";

                BookingImportResultDto bResult = new BookingImportResultDto();
                bResult.WarehouseCode = bookingHeader.WarehouseCode;
                bResult.BookingId = bookingHeader.BookingId;
                bResult.OperationType = bookingHeader.MerchType;
                bResult.TimeSlot = bookingHeader.BookingStart;
                bResult.TotalQty = bookingHeader.BookingDetails.Sum(x => x.Weight).Value;
                bResult.TotalTruck = bookingHeader.BookingTrucks.Sum(x=>x.TotalTruck).Value;
                bhResult.Add(bResult);
            }
            else
            {
                if (overCutoff && operation.Overcutoff.Value)
                {
                    bookingHeader.Status = "OVERCUTOFF";
                }

                // booking same excel import
                BookingImportResultDto bResult = new BookingImportResultDto();
                bResult.WarehouseCode = bookingHeader.WarehouseCode;
                bResult.BookingId = bookingHeader.BookingId;
                bResult.OperationType = bookingHeader.MerchType;
                bResult.TimeSlot = bookingHeader.BookingStart;
                bResult.TotalQty = bookingHeader.BookingDetails.Sum(x => x.Weight).Value;
                bResult.TotalTruck = bookingHeader.BookingTrucks.Sum(x => x.TotalTruck).Value;
                bhResult.Add(bResult);
            }
            
            return bookingHeader;
        }

        public async Task<CreateBookingExcelDto> ImportOld(CreateBookingExcelDto createBookingExcelDto,string userName)
        {
            List<PoList> poLists = new List<PoList>();
            int non = 0;
            int con = 0;
            int full = 0;
            int total = 0;

            List<BookingHeaderDto> bookingHeaderDtos = new List<BookingHeaderDto>();
            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            bool saveBooking = true;

            Supplier supplier = unitOfWork.SupplierRepository.GetSupplierBySupCode(createBookingExcelDto.SupplierCode);
            SupplierGroup supplierGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(createBookingExcelDto.InternalSupGroupId);

            var listWhse = new List<string>();
            var listWhseError = new List<string>();
            var listWhseOverCap = new List<string>();
            var listWhseUpdate = new List<string>();

            // get warehouse
            var whses = unitOfWork.WarehouseRepository.GetWarehouses().ToList();

            createBookingExcelDto.BookingHeaders = new List<BookingHeader>();

            // get estimate time            
            var estTimes = unitOfWork.EstTimeRepository.GetEstTimeBySupGroup(createBookingExcelDto.InternalSupGroupId).ToList();

            // get truck
            var trucks = unitOfWork.TruckMasterRepository.GetTrucks().ToList();

            if (poLists.Count > 0)
            {                
                CheckWarehouse(ref createBookingExcelDto, poLists, whses, ref listWhse,estTimes,trucks);
            }
            else
            {
                return createBookingExcelDto;
            }

            bookingHeaderDtos = CreateBookingDto(ref createBookingExcelDto,poLists,whses,ref listWhse,estTimes,trucks);

            // check warehouse Capacity
            foreach (var item in bookingHeaderDtos)
            {
                var whseCode = whses.FirstOrDefault(x => x.WarehouseCode == item.WarehouseCode && x.CompanyCode == item.CompanyCode);

                if(whseCode.CapacityType == "C")
                {
                    var whseCap = await warehouseCapacityService.GetCapacityByBookingDate(createBookingExcelDto.BookingDate, whseCode.WarehouseCode);
                    if(whseCap == null)
                    {
                        createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList()
                            .ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                        {                            
                            bookWhse.ImportResult = "Over capacity";
                        });
                        listWhseOverCap.Add(whseCode.WarehouseCode);
                    }
                    else
                    {
                        var checkCaps = whseCap.Where(x => x.BookingDateTime >= item.BookingStart && x.BookingDateTime <= item.BookingEnd).ToList();

                        foreach (var cap in checkCaps)
                        {
                            if (cap.CUBE_CON / cap.MAX_CUBE_CON * 100 > 100)
                            {
                                createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList()
                                    .ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                                {
                                    bookWhse.ImportResult = "Over capacity";
                                });
                                listWhseOverCap.Add(whseCode.WarehouseCode);
                            }
                            else
                            {
                                // get cap by warehouse

                                var cubeCon = item.BookingDetails.Sum(x => x.CubeCon);

                                if ((cap.CUBE_CON + cubeCon) / cap.MAX_CUBE_CON * 100 > 100)
                                {
                                    createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList()
                                        .ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                                    {
                                        bookWhse.ImportResult = "Over capacity";
                                    });
                                    listWhseOverCap.Add(whseCode.WarehouseCode);
                                }
                            }
                        }
                    }
                }

                if(whseCode.CapacityType == "W")
                {
                    var whseCap = await warehouseCapacityService.GetOperationCapacityByBookingDate(createBookingExcelDto.BookingDate, whseCode.WarehouseCode,item.MerchType);
                    if (whseCap == null)
                    {
                        createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList()
                            .ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                            {
                                bookWhse.ImportResult = "Over capacity";
                            });
                    }
                    else
                    {
                        var checkCaps = whseCap.Where(x => x.BookingDateTime >= item.BookingStart && x.BookingDateTime <= item.BookingEnd).ToList();
                        foreach (var cap in checkCaps)
                        {
                            if (cap.FULL_PL == cap.CUBE_FULL)
                            {
                                createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList()
                                    .ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                                    {
                                        bookWhse.ImportResult = "Over capacity";
                                    });
                                listWhseOverCap.Add(whseCode.WarehouseCode);
                            }                            
                        }
                    }
                }
            }

            // update booking
            foreach (var item in bookingHeaderDtos.Where(x => x.BookingId != null))
            {
                var isUpdate = false;

                var whseInfo = whses.FirstOrDefault(x => x.WarehouseCode == item.WarehouseCode && x.CompanyCode == item.CompanyCode);
                
                var bookingHdrInfo = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(item.BookingId);

                var bookingTime = bookingHdrInfo.BookingEnd - bookingHdrInfo.BookingStart;
                var estimateTime = item.BookingEnd - item.BookingStart;

                List<string> statuses = new List<string>();
                statuses.Add("NEW");
                statuses.Add("APPROVED");
                if (statuses.Contains(bookingHdrInfo.Status))
                {
                    if (estimateTime.TotalMinutes > bookingTime.Value.TotalMinutes)
                    {
                        if (whseInfo.FixDoor == "Y")
                        {
                            // check dock door available time slot
                            string doorType = supplierGroup == null ? "" : supplierGroup.Remark!;

                            var doors = unitOfWork.DoorRepository.GetDoorAvailable(createBookingExcelDto.BookingDate, bookingHdrInfo.BookingStart.Value
                                , bookingHdrInfo.BookingEnd.Value, whseInfo.WarehouseCode, doorType, bookingHdrInfo.BookingId)
                                .OrderBy(x => x.Sequence);

                            if (doors.ToList().Count == 0)
                            {
                                createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseInfo.WarehouseMain)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                                {
                                    bookWhse.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
                                });

                                foreach (var po in poLists.Where(x => x.Warehouse_Code == whseInfo.WarehouseCode))
                                {
                                    //createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
                                    //{
                                    //    bookPo.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
                                    //});
                                }
                            }
                            else
                            {
                                var door = doors.FirstOrDefault(x => x.InternalDoorId == bookingHdrInfo.InternalDoorId);

                                if (door == null)
                                {
                                    // update door
                                    bookingHdrInfo.InternalDoorId = doors.FirstOrDefault().InternalDoorId;

                                    unitOfWork.BookingHeaderRepository.Update(bookingHdrInfo);

                                    unitOfWork.Save();

                                    isUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            isUpdate = true;
                        }
                    }
                    else
                    {
                        isUpdate = true;
                    }
                    if (isUpdate)
                    {
                        // update booking detail only
                        bookingHdrInfo.BookingStart = item.BookingStart;
                        bookingHdrInfo.BookingEnd = item.BookingEnd;
                        bookingHdrInfo.ModDate = DateTime.Now;
                        bookingHdrInfo.UserStamp = userName;
                        bookingHdrInfo.Status = "NEW";

                        foreach (var po in item.BookingDetails)
                        {
                            
                            var bookingDtl = new BookingDetail();

                            var bookingDtlId = unitOfWork.PoListRepository.GetBookingDetailKey();

                            unitOfWork.Save();

                            //var poRemark = createBookingExcelDto.CreateBookingExcelPos.Find(x => x.PoNo == po.PoNbr).Remark;

                            //bookingDtl.PoNbr = po.PoNbr;
                            //bookingDtl.TotalQty = po.TotalQty;
                            //bookingDtl.Con = po.Con;
                            //bookingDtl.Non = po.Non;
                            //bookingDtl.FullPl = po.FullPl;
                            //bookingDtl.CubeCon = po.CubeCon;
                            //bookingDtl.CubeNon = po.CubeNon;
                            //bookingDtl.CubeFull = po.CubeFull;
                            //bookingDtl.PlanRec = po.PlanRec;
                            //bookingDtl.Postponed = po.Postponed;
                            //bookingDtl.InternalDetailKey = bookingDtlId;
                            //bookingDtl.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;
                            //bookingDtl.UserStamp = userName;
                            //bookingDtl.Status = "NEW";
                            //bookingDtl.CreateDate = DateTime.Now;
                            //bookingDtl.ModDate = DateTime.Now;
                            //bookingDtl.Remark = poRemark;
                            //bookingDtl.Weight = po.Weight;
                            //bookingDtl.MerchType = po.MerchType;

                            //createBookingExcelDto.CreateBookingExcelPos.Find(x => x.PoNo == po.PoNbr).ImportResult = "Booking ID: " + bookingHdrInfo.BookingId;

                            unitOfWork.Save();

                            unitOfWork.BookingDetailRepository.Add(bookingDtl);
                            unitOfWork.BookingHeaderRepository.Update(bookingHdrInfo);

                            unitOfWork.Save();
                        }

                        // update truck type
                        var bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingHdrInfo.InternalHeaderKey).ToList();

                        // create truck
                        foreach (var bookingTruckDto in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == whseInfo.WarehouseCode))
                        {
                            var checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "4 W").InternalTruckId);

                            //if (checkTruck == null)
                            //{
                            //    var bookingTruck = new BookingTruck();
                            //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "4 W").InternalTruckId;
                            //    bookingTruck.TotalTruck = bookingTruckDto.FourwheelQty;
                            //    bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

                            //    if (bookingTruckDto.FourwheelQty > 0)
                            //    {
                            //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                            //    }
                            //}
                            //else
                            //{
                            //    bookingTruckDto.FourwheelQty = checkTruck.TotalTruck.Value;
                            //}

                            //checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "6 W").InternalTruckId);

                            //if (checkTruck == null)
                            //{
                            //    var bookingTruck = new BookingTruck();
                            //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "6 W").InternalTruckId;
                            //    bookingTruck.TotalTruck = bookingTruckDto.SixwheelQty;
                            //    bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

                            //    if (bookingTruckDto.SixwheelQty > 0)
                            //    {
                            //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                            //    }
                            //}
                            //else
                            //{
                            //    bookingTruckDto.SixwheelQty = checkTruck.TotalTruck.Value;
                            //}

                            //checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "10 W").InternalTruckId);

                            //if (checkTruck == null)
                            //{
                            //    var bookingTruck = new BookingTruck();
                            //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "10 W").InternalTruckId;
                            //    bookingTruck.TotalTruck = bookingTruckDto.TenwheelQty;
                            //    bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

                            //    if (bookingTruckDto.TenwheelQty > 0)
                            //    {
                            //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                            //    }
                            //}
                            //else
                            //{
                            //    bookingTruckDto.TenwheelQty = checkTruck.TotalTruck.Value;
                            //}

                            //checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "18 W").InternalTruckId);

                            //if (checkTruck == null)
                            //{
                            //    var bookingTruck = new BookingTruck();
                            //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "18 W").InternalTruckId;
                            //    bookingTruck.TotalTruck = bookingTruckDto.EighteenwheelQty;
                            //    bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

                            //    if (bookingTruckDto.EighteenwheelQty > 0)
                            //    {
                            //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                            //    }
                            //}
                            //else
                            //{
                            //    bookingTruckDto.EighteenwheelQty = checkTruck.TotalTruck.Value;
                            //}


                            unitOfWork.Save();
                        }

                        createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == whseInfo.WarehouseCode).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                        {
                            bookWhse.ImportResult = "Completed";
                        });

                        if (bookingHdrInfo.InternalKeyId > 0)
                        {
                            var bookingHdrs = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingHdrInfo.InternalKeyId.Value).ToList();


                            foreach (var bh in bookingHdrs)
                            {
                                createBookingExcelDto.BookingHeaders.Add(bh);
                            }

                        }
                    }
                }
                else
                {
                    createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseInfo.WarehouseCode)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                    {
                        bookWhse.ImportResult = "Cannot add PO or change booking time of Booking ID " + bookingHdrInfo.BookingId + " because this booking finished the booking process." +
                        "ไม่สามารถเพิ่ม PO เข้าไปยัง Booking ID " + bookingHdrInfo.BookingId + " เนื่องจากมีสถานะจบการ Booking แล้ว";

                    });

                    foreach (var po in poLists.Where(x => x.Warehouse_Code == whseInfo.WarehouseCode))
                    {
                        //createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
                        //{
                        //    bookPo.ImportResult = "Cannot add PO or change booking time of Booking ID " + bookingHdrInfo.BookingId + " because this booking finished the booking process." +
                        //"ไม่สามารถเพิ่ม PO เข้าไปยัง Booking ID " + bookingHdrInfo.BookingId + " เนื่องจากมีสถานะจบการ Booking แล้ว";
                        //});
                    }
                }            

            }

            BookingKey bookingKey = new BookingKey();
            int bookingKeyId = 0;

            if (bookingHeaderDtos.Count(x=>x.BookingId == null) > 0)
            {
                BookingHeader bookingHdr = new BookingHeader();
                BookingDetail bookingDtl = new BookingDetail();
                BookingTruck bookingTruck = new BookingTruck();

                // create booking key
                bookingKeyId = unitOfWork.PoListRepository.GetBookingKey();
                
                bookingKey.InternalKeyId = bookingKeyId;
                bookingKey.BookingDate = createBookingExcelDto.BookingDate.ToLocalTime().Date;
                bookingKey.CreateDate = DateTime.Now;
                bookingKey.Active = true;
                bookingKey.CompanyCode = bookingHeaderDtos[0].CompanyCode;
                bookingKey.UserStamp = userName;
                bookingKey.ModDate = DateTime.Now;

                var saveBookingKey = await Task.Run<bool>(() => unitOfWork.BookingKeyRepository.Add(bookingKey));

            }

            foreach (var item in bookingHeaderDtos.Where(x => x.BookingId == null))
            {
                var isCanSave = true;
                BookingHeader bookingHdr = new BookingHeader();
                BookingDetail bookingDtl = new BookingDetail();
                BookingTruck bookingTruck = new BookingTruck();

                var bookingStart = item.BookingStart;
                var bookingEnd = item.BookingEnd;
                string doorType = supplierGroup == null ? "" : supplierGroup.Remark!;

                var whseCode = whses.FirstOrDefault(x => x.WarehouseCode == item.WarehouseCode && x.CompanyCode == item.CompanyCode) ;

                var doors = unitOfWork.DoorRepository.GetDoorAvailable(bookingKey.BookingDate.Value, bookingStart, bookingEnd, item.WarehouseCode, doorType).OrderBy(x => x.Sequence);

                if (whseCode.FixDoor == "Y")
                {                    
                    if(doors.ToList().Count == 0)
                    {
                        createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList()
                            .ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                        {
                            bookWhse.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
                            isCanSave = false;
                        });

                        //foreach (var po in poLists.Where(x => x.Warehouse_Code == whseCode.WarehouseMain))
                        //{
                        //    createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList()
                        //        .ForEach(delegate (CreateBookingExcelPo bookPo)
                        //    {
                        //        bookPo.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
                        //        isCanSave = false;
                        //    });
                        //}
                    }
                }

                if (isCanSave)
                {
                    unitOfWork.Save();
                    // create booking header
                    string bookingId = "";

                    // generate booking header id
                    var warehouse = whses.FirstOrDefault(x => x.WarehouseCode == item.WarehouseCode && x.CompanyCode == item.CompanyCode);

                    if (warehouse.OnlineBookingIdRunning == null)
                    {
                        warehouse.OnlineBookingIdRunning = 1;
                    }
                    else
                    {
                        warehouse.OnlineBookingIdRunning += 1;
                    }
                    
                    bookingId = warehouse.OnlineBookingIdPrefix + DateTime.Now.Date.ToString("MMdd")
                       + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');
                    
                    unitOfWork.WarehouseRepository.Update(warehouse);

                    unitOfWork.Save();

                    bookingHdr = new BookingHeader();
                    bookingHdr.InternalHeaderKey = unitOfWork.PoListRepository.GetBookingHeaderKey();
                    bookingHdr.InternalKeyId = bookingKey.InternalKeyId;
                    bookingHdr.InternalSupGroupId = createBookingExcelDto.InternalSupGroupId;

                    if (whseCode.FixDoor == "Y")
                    {
                        bookingHdr.InternalDoorId = doors.FirstOrDefault().InternalDoorId;
                    }
                    else
                    {
                        bookingHdr.InternalDoorId = 0;
                    }
                    
                    bookingHdr.BookingId = bookingId;
                    bookingHdr.BookingStart = item.BookingStart;
                    bookingHdr.BookingEnd = item.BookingEnd;
                    bookingHdr.FirstBookginStart = item.BookingStart;
                    bookingHdr.FirstBookingEnd = item.BookingEnd;
                    bookingHdr.ContactName = createBookingExcelDto.ContactName;
                    bookingHdr.ContactEmail = createBookingExcelDto.ContactEmail;
                    bookingHdr.ContactTel = createBookingExcelDto.ContactPhone;
                    bookingHdr.WarehouseCode = warehouse.WarehouseCode;
                    bookingHdr.CompanyCode = warehouse.CompanyCode;
                    bookingHdr.SupCode = supplier.SupCode;
                    bookingHdr.SupName = supplier.SupName;
                    bookingHdr.TotalPo = item.BookingDetails.Count();
                    bookingHdr.TotalQty = item.BookingDetails.Sum(x => x.TotalQty);
                    bookingHdr.BackHaul = false;
                    bookingHdr.Active = true;
                    bookingHdr.Status = "NEW";
                    bookingHdr.UserStamp = userName;
                    bookingHdr.CreateDate = DateTime.Now;
                    bookingHdr.Remark = "";
                    bookingHdr.ModDate = DateTime.Now;
                    bookingHdr.MerchType = item.MerchType;

                    bookingHdr.Postponed = false;

                    var countPostponed = item.BookingDetails.Count(x => x.Postponed.ToUpper() == "Y");

                    if (countPostponed > 0)
                    {
                        if (item.BookingDetails.Count(x => x.PlanRec.Value.Date != bookingHdr.FirstBookginStart.Value.Date) > 0)
                        {
                            bookingHdr.Postponed = true;
                            bookingHdr.ApproveCondition = "POSTPONED";
                        }
                    }

                    unitOfWork.BookingHeaderRepository.Add(bookingHdr);
                    unitOfWork.Save();

                    //foreach (var po in item.BookingDetails)
                    //{
                    //    var poRemark = createBookingExcelDto.CreateBookingExcelPos.Find(x => x.PoNo == po.PoNbr).Remark;

                    //    bookingDtl = new BookingDetail();

                    //    bookingDtl.PoNbr = po.PoNbr;
                    //    bookingDtl.TotalQty = po.TotalQty;
                    //    bookingDtl.Con = po.Con;
                    //    bookingDtl.Non = po.Non;
                    //    bookingDtl.FullPl = po.FullPl;
                    //    bookingDtl.CubeCon = po.CubeCon;
                    //    bookingDtl.CubeNon = po.CubeNon;
                    //    bookingDtl.CubeFull = po.CubeFull;
                    //    bookingDtl.PlanRec = po.PlanRec;
                    //    bookingDtl.Postponed = po.Postponed;
                    //    bookingDtl.InternalDetailKey = unitOfWork.PoListRepository.GetBookingDetailKey();
                    //    bookingDtl.InternalHeaderKey = bookingHdr.InternalHeaderKey;
                    //    bookingDtl.UserStamp = userName;
                    //    bookingDtl.Status = "NEW";
                    //    bookingDtl.CreateDate = DateTime.Now;
                    //    bookingDtl.ModDate = DateTime.Now;
                    //    bookingDtl.Remark = poRemark;
                    //    bookingDtl.Weight = po.Weight;
                    //    bookingDtl.MerchType = po.MerchType;

                    //    unitOfWork.BookingDetailRepository.Add(bookingDtl);
                    //}

                    //foreach (var bookingTruckDto in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == whseCode.WarehouseCode))
                    //{
                    //    bookingTruck = new BookingTruck();
                    //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "4 W").InternalTruckId;
                    //    bookingTruck.TotalTruck = bookingTruckDto.FourwheelQty;
                    //    bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

                    //    if (bookingTruckDto.FourwheelQty > 0)
                    //    {
                    //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                    //    }

                    //    bookingTruck = new BookingTruck();
                    //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "6 W").InternalTruckId;
                    //    bookingTruck.TotalTruck = bookingTruckDto.SixwheelQty;
                    //    bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

                    //    if (bookingTruckDto.SixwheelQty > 0)
                    //    {
                    //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                    //    }

                    //    bookingTruck = new BookingTruck();
                    //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "10 W").InternalTruckId;
                    //    bookingTruck.TotalTruck = bookingTruckDto.TenwheelQty;
                    //    bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

                    //    if (bookingTruckDto.TenwheelQty > 0)
                    //    {
                    //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                    //    }

                    //    bookingTruck = new BookingTruck();
                    //    bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "18 W").InternalTruckId;
                    //    bookingTruck.TotalTruck = bookingTruckDto.EighteenwheelQty;
                    //    bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

                    //    if (bookingTruckDto.EighteenwheelQty > 0)
                    //    {
                    //        unitOfWork.BookingTruckRepository.Add(bookingTruck);
                    //    }

                    //}

                    //unitOfWork.Save();

                    //createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseMain)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
                    //{
                    //    bookWhse.ImportResult = "Completed.";
                    //});

                    //foreach (var po in item.BookingDetails)
                    //{
                    //    createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.PoNbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
                    //    {
                    //        bookPo.ImportResult = "Booking ID : " + bookingId;
                    //    });
                    //}
                }

                if (bookingKeyId > 0)
                {
                    var bookingHdrs = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingKeyId).ToList();


                    foreach (var bh in bookingHdrs)
                    {
                        createBookingExcelDto.BookingHeaders.Add(bh);
                    }

                }
            }


            #region ++ Old code ++

            //// update booking
            //foreach (var whse in listWhseUpdate)
            //{
            //    var isUpdate = true;
            //    var whseInfo = whses.FirstOrDefault(x => x.WarehouseCode == whse);

            //    var poExcelLists = createBookingExcelDto.CreateBookingExcelPos.Where(x => x.WarehouseCode == whse).ToList();

            //    var bookingHdr = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse);

            //    var bookingHdrInfo = unitOfWork.BookingHeaderRepository.GetBookingHeadersByBookingId(bookingHdr.BookingId);

            //    var bookingTime = bookingHdrInfo.BookingEnd - bookingHdrInfo.BookingStart;
            //    var estimateTime = bookingHdr.BookingEnd - bookingHdr.BookingStart;

            //    if(bookingTime.Value.TotalMinutes >= estimateTime.TotalMinutes)
            //    {

            //    }
            //    else
            //    {
            //        // check dock door available time slot
            //        string doorType = supplierGroup == null ? "" : supplierGroup.Remark!;

            //        var doors = unitOfWork.DoorRepository.GetDoorAvailable(createBookingExcelDto.BookingDate, bookingHdrInfo.BookingStart.Value, bookingHdrInfo.BookingEnd.Value, whseInfo.WarehouseCode, doorType,bookingHdrInfo.BookingId)
            //            .OrderBy(x => x.Sequence);

            //        if (doors.ToList().Count == 0)
            //        {
            //            createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseInfo.WarehouseMain)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
            //            {
            //                bookWhse.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
            //            });

            //            foreach (var po in poLists.Where(x => x.Warehouse_Code == whseInfo.WarehouseCode))
            //            {
            //                createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
            //                {
            //                    bookPo.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
            //                });
            //            }

            //            isUpdate = false;
            //        }
            //        else
            //        {
            //            var door = doors.FirstOrDefault(x => x.InternalDoorId == bookingHdrInfo.InternalDoorId);

            //            if (door == null)
            //            {
            //                // update door
            //                bookingHdrInfo.InternalDoorId = doors.FirstOrDefault().InternalDoorId;

            //                unitOfWork.BookingHeaderRepository.Update(bookingHdrInfo);

            //                unitOfWork.Save();
            //            }                        
            //        }
            //    }

            //    List<string> statuses = new List<string>();
            //    statuses.Add("NEW");
            //    statuses.Add("APPROVED");
            //    if (statuses.Contains(bookingHdrInfo.Status))
            //    {
            //        isUpdate = true;
            //    }
            //    else
            //    {
            //        createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseInfo.WarehouseCode)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
            //        {
            //            bookWhse.ImportResult = "Cannot add PO or change booking time of Booking ID " + bookingHdrInfo.BookingId + " because this booking finished the booking process." +
            //            "ไม่สามารถเพิ่ม PO เข้าไปยัง Booking ID " + bookingHdrInfo.BookingId + " เนื่องจากมีสถานะจบการ Booking แล้ว";

            //        });

            //        foreach (var po in poLists.Where(x => x.Warehouse_Code == whseInfo.WarehouseCode))
            //        {
            //            createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
            //            {
            //                bookPo.ImportResult = "Cannot add PO or change booking time of Booking ID " + bookingHdrInfo.BookingId + " because this booking finished the booking process." +
            //            "ไม่สามารถเพิ่ม PO เข้าไปยัง Booking ID " + bookingHdrInfo.BookingId + " เนื่องจากมีสถานะจบการ Booking แล้ว";
            //            });
            //        }
            //        isUpdate = false;
            //    }

            //    if (isUpdate)
            //    {
            //        // update booking detail only
            //        bookingHdrInfo.BookingStart = bookingHdr.BookingStart;
            //        bookingHdrInfo.BookingEnd = bookingHdr.BookingEnd;
            //        bookingHdrInfo.ModDate = DateTime.Now;
            //        bookingHdrInfo.UserStamp = userName;
            //        bookingHdrInfo.Status = "NEW";

            //        foreach (var poBooking in poExcelLists.Where(x => x.ImportResult == "").ToList())
            //        {
            //            var po = unitOfWork.PoListRepository.GetPoByPoNo(poBooking.PoNo, createBookingExcelDto.SupplierCode, "88");

            //            var bookingDtl = new BookingDetail();

            //            var bookingDtlId = unitOfWork.PoListRepository.GetBookingDetailKey();

            //            unitOfWork.Save();

            //            var poRemark = createBookingExcelDto.CreateBookingExcelPos.Find(x => x.PoNo == po.Po_Nbr).Remark;

            //            bookingDtl.PoNbr = po.Po_Nbr;
            //            bookingDtl.TotalQty = po.Total_Qty;
            //            bookingDtl.Con = po.Con;
            //            bookingDtl.Non = po.Non;
            //            bookingDtl.FullPl = po.Full;
            //            bookingDtl.CubeCon = po.Cube_Con;
            //            bookingDtl.CubeNon = po.Cube_Non;
            //            bookingDtl.CubeFull = po.Cube_Full;
            //            bookingDtl.PlanRec = po.Plan_Receive_Date;
            //            bookingDtl.Postponed = po.PostPoned;
            //            bookingDtl.InternalDetailKey = bookingDtlId;
            //            bookingDtl.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;
            //            bookingDtl.UserStamp = userName;
            //            bookingDtl.Status = "NEW";
            //            bookingDtl.CreateDate = DateTime.Now;
            //            bookingDtl.ModDate = DateTime.Now;
            //            bookingDtl.Remark = poRemark;
            //            bookingDtl.Weight = po.Weight;
            //            bookingDtl.MerchType = po.Merch_Type;

            //            poBooking.ImportResult = "Booking ID: " + bookingHdrInfo.BookingId;

            //            unitOfWork.Save();

            //            unitOfWork.BookingDetailRepository.Add(bookingDtl);
            //            unitOfWork.BookingHeaderRepository.Update(bookingHdrInfo);

            //            unitOfWork.Save();
            //        }

            //        // update truck type
            //        var bookingTrucks = unitOfWork.BookingTruckRepository.GetBookingTrucksByHeaderId(bookingHdrInfo.InternalHeaderKey).ToList();

            //        // create truck
            //        foreach (var bookingTruckDto in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == whseInfo.WarehouseCode))
            //        {


            //            var checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "4 W").InternalTruckId);

            //            if (checkTruck == null)
            //            {
            //                var bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "4 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.FourwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

            //                if (bookingTruckDto.FourwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }
            //            }
            //            else
            //            {
            //                bookingTruckDto.FourwheelQty = checkTruck.TotalTruck.Value;
            //            }

            //            checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "6 W").InternalTruckId);

            //            if (checkTruck == null)
            //            {
            //                var bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "6 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.SixwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

            //                if (bookingTruckDto.SixwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }
            //            }
            //            else
            //            {
            //                bookingTruckDto.SixwheelQty = checkTruck.TotalTruck.Value;
            //            }

            //            checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "10 W").InternalTruckId);

            //            if (checkTruck == null)
            //            {
            //                var bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "10 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.TenwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

            //                if (bookingTruckDto.TenwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }
            //            }
            //            else
            //            {
            //                bookingTruckDto.TenwheelQty = checkTruck.TotalTruck.Value;
            //            }

            //            checkTruck = bookingTrucks.FirstOrDefault(x => x.InternalTruckId == trucks.FirstOrDefault(x => x.TruckCode == "18 W").InternalTruckId);

            //            if (checkTruck == null)
            //            {
            //                var bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "18 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.EighteenwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdrInfo.InternalHeaderKey;

            //                if (bookingTruckDto.EighteenwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }
            //            }
            //            else
            //            {
            //                bookingTruckDto.EighteenwheelQty = checkTruck.TotalTruck.Value;
            //            }


            //            unitOfWork.Save();
            //        }

            //        createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == whseInfo.WarehouseCode).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
            //        {
            //            bookWhse.ImportResult = "Completed";
            //        });

            //        if (bookingHdrInfo.InternalKeyId > 0)
            //        {
            //            var bookingHdrs = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingHdrInfo.InternalKeyId.Value).ToList();


            //            foreach (var item in bookingHdrs)
            //            {
            //                createBookingExcelDto.BookingHeaders.Add(item);
            //            }

            //        }
            //    }                

            //}

            #endregion

            #region +++ Old save code +++

            //if (saveBooking)
            //{

            //    BookingKey bookingKey = new BookingKey();
            //    BookingHeader bookingHdr = new BookingHeader();
            //    BookingDetail bookingDtl = new BookingDetail();
            //    BookingTruck bookingTruck = new BookingTruck();

            //    // create booking key
            //    var bookingKeyId = unitOfWork.PoListRepository.GetBookingKey();

            //    bookingKey.InternalKeyId = bookingKeyId;
            //    bookingKey.BookingDate = createBookingExcelDto.BookingDate.ToLocalTime().Date;
            //    bookingKey.CreateDate = DateTime.Now;
            //    bookingKey.Active = true;
            //    bookingKey.CompanyCode = "88";
            //    bookingKey.UserStamp = userName;
            //    bookingKey.ModDate = DateTime.Now;

            //    var saveBookingKey = await Task.Run<bool>(() => unitOfWork.BookingKeyRepository.Add(bookingKey));

            //    foreach (var whse in listWhse)
            //    {
            //        // find door available
            //        var bookingStart = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse).BookingStart;
            //        var bookingEnd = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse).BookingEnd;
            //        string doorType = supplierGroup == null ? "" : supplierGroup.Remark!;

            //        var whseCode = whses.FirstOrDefault(x => x.WarehouseCode == whse);

            //        var doors = unitOfWork.DoorRepository.GetDoorAvailable(bookingKey.BookingDate.Value, bookingStart, bookingEnd, whse, doorType).OrderBy(x=>x.Sequence);

            //        if (doors.ToList().Count == 0)
            //        {
            //            createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseCode)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
            //            {
            //                bookWhse.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
            //            });

            //            foreach (var po in poLists.Where(x => x.Warehouse_Code == whseCode.WarehouseMain))
            //            {
            //                createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
            //                {
            //                    bookPo.ImportResult = "No dock door available, Please contact admin. ไม่มีประตูลงสินค้าว่าง โปรดติดต่อเจ้าหน้าที่";
            //                });
            //            }
            //         }
            //        else
            //        {

            //            unitOfWork.Save();

            //            // create booking header
            //            string bookingId = "";

            //            // generate booking header id
            //            var warehouse = whses.FirstOrDefault(x => x.WarehouseCode == whse);

            //            if (warehouse.OnlineBookingIdRunning == null)
            //            {
            //                warehouse.OnlineBookingIdRunning = 1;
            //            }
            //            else
            //            {
            //                warehouse.OnlineBookingIdRunning += 1;
            //            }

            //            //bookingId = warehouse.OnlineBookingIdPrefix + createBookingExcelDto.BookingDate.ToLocalTime().Date.ToString("MMdd") + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');
            //            bookingId = warehouse.OnlineBookingIdPrefix + DateTime.Now.Date.ToString("MMdd") 
            //                + warehouse.OnlineBookingIdRunning.Value.ToString().PadLeft(4, '0');


            //            unitOfWork.WarehouseRepository.Update(warehouse);

            //            unitOfWork.Save();

            //            bookingHdr = new BookingHeader();
            //            bookingHdr.InternalHeaderKey = unitOfWork.PoListRepository.GetBookingHeaderKey();
            //            bookingHdr.InternalKeyId = bookingKey.InternalKeyId;
            //            bookingHdr.InternalSupGroupId = createBookingExcelDto.InternalSupGroupId;
            //            bookingHdr.InternalDoorId = doors.FirstOrDefault().InternalDoorId;
            //            bookingHdr.BookingId = bookingId;
            //            bookingHdr.BookingStart = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse).BookingStart;
            //            bookingHdr.BookingEnd = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse).BookingEnd;
            //            bookingHdr.FirstBookginStart = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse).BookingStart;
            //            bookingHdr.FirstBookingEnd = bookingHeaderDtos.FirstOrDefault(x => x.WarehouseCode == whse).BookingEnd;
            //            bookingHdr.ContactName = createBookingExcelDto.ContactName;
            //            bookingHdr.ContactEmail = createBookingExcelDto.ContactEmail;
            //            bookingHdr.ContactTel = createBookingExcelDto.ContactPhone;
            //            bookingHdr.WarehouseCode = whse;
            //            bookingHdr.SupCode = supplier.SupCode;
            //            bookingHdr.SupName = supplier.SupName;
            //            bookingHdr.TotalPo = poLists.Count(x => x.Warehouse_Code == whse);
            //            bookingHdr.TotalQty = poLists.Where(x => x.Warehouse_Code == whse).Sum(x => x.Total_Qty);
            //            bookingHdr.BackHaul = false;
            //            bookingHdr.Active = true;
            //            bookingHdr.Status = "NEW";
            //            bookingHdr.UserStamp = userName;
            //            bookingHdr.CreateDate = DateTime.Now;
            //            bookingHdr.Remark = "";
            //            bookingHdr.ModDate = DateTime.Now;
            //            bookingHdr.MerchType = poLists.Select(x => x.Merch_Type).FirstOrDefault();

            //            bookingHdr.Postponed = false;

            //            var countPostponed = poLists.Count(x => x.PostPoned.ToUpper() == "Y");

            //            if (countPostponed > 0)
            //            {
            //                if (poLists.Count(x => x.Plan_Receive_Date.Date != bookingHdr.FirstBookginStart.Value.Date) > 0)
            //                {
            //                    bookingHdr.Postponed = true;
            //                    bookingHdr.ApproveCondition = "POSTPONED";
            //                }
            //            }

            //            unitOfWork.BookingHeaderRepository.Add(bookingHdr);
            //            unitOfWork.Save();

            //            // create booking detail
            //            foreach (var po in poLists.Where(x => x.Warehouse_Code == whse))
            //            {
            //                var poRemark = createBookingExcelDto.CreateBookingExcelPos.Find(x => x.PoNo == po.Po_Nbr).Remark;

            //                bookingDtl = new BookingDetail();

            //                bookingDtl.PoNbr = po.Po_Nbr;
            //                bookingDtl.TotalQty = po.Total_Qty;
            //                bookingDtl.Con = po.Con;
            //                bookingDtl.Non = po.Non;
            //                bookingDtl.FullPl = po.Full;
            //                bookingDtl.CubeCon = po.Cube_Con;
            //                bookingDtl.CubeNon = po.Cube_Non;
            //                bookingDtl.CubeFull = po.Cube_Full;
            //                bookingDtl.PlanRec = po.Plan_Receive_Date;
            //                bookingDtl.Postponed = po.PostPoned;
            //                bookingDtl.InternalDetailKey = unitOfWork.PoListRepository.GetBookingDetailKey();
            //                bookingDtl.InternalHeaderKey = bookingHdr.InternalHeaderKey;
            //                bookingDtl.UserStamp = userName;
            //                bookingDtl.Status = "NEW";
            //                bookingDtl.CreateDate = DateTime.Now;
            //                bookingDtl.ModDate = DateTime.Now;
            //                bookingDtl.Remark = poRemark;
            //                bookingDtl.Weight = po.Weight;
            //                bookingDtl.MerchType = po.Merch_Type;

            //                unitOfWork.BookingDetailRepository.Add(bookingDtl);
            //            }

            //            // create truck
            //            foreach (var bookingTruckDto in createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == whseCode.WarehouseMain))
            //            {
            //                bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "4 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.FourwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

            //                if (bookingTruckDto.FourwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }

            //                bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "6 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.SixwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

            //                if (bookingTruckDto.SixwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }

            //                bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "10 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.TenwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

            //                if (bookingTruckDto.TenwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }

            //                bookingTruck = new BookingTruck();
            //                bookingTruck.InternalTruckId = trucks.FirstOrDefault(x => x.TruckCode == "18 W").InternalTruckId;
            //                bookingTruck.TotalTruck = bookingTruckDto.EighteenwheelQty;
            //                bookingTruck.InternalHeaderKey = bookingHdr.InternalHeaderKey;

            //                if (bookingTruckDto.EighteenwheelQty > 0)
            //                {
            //                    unitOfWork.BookingTruckRepository.Add(bookingTruck);
            //                }

            //            }

            //            unitOfWork.Save();

            //            createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode.Equals(whseCode.WarehouseMain)).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
            //            {
            //                bookWhse.ImportResult = "Completed.";
            //            });

            //            foreach (var po in poLists.Where(x => x.Warehouse_Code == whse))
            //            {
            //                createBookingExcelDto.CreateBookingExcelPos.Where(x => x.PoNo == po.Po_Nbr).ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
            //                {
            //                    bookPo.ImportResult = "Booking ID : " + bookingId;
            //                });
            //            }
            //        }
            //    }

            // get booking header by booking key

            //if (bookingKeyId > 0)
            //{
            //    var bookingHdrs = unitOfWork.BookingHeaderRepository.GetBookingHeaderByKeyId(bookingKeyId).ToList();


            //    foreach (var item in bookingHdrs)
            //    {
            //        if (listWhse.Contains(item.WarehouseCode) || listWhseUpdate.Contains(item.WarehouseCode))
            //        {
            //            createBookingExcelDto.BookingHeaders.Add(item);
            //        }
            //    }

            //}
            //}

            #endregion


            //foreach (var whseError in listWhseError.Distinct())
            //{
            //    var warehouse = whses.FirstOrDefault(x => x.WarehouseCode == whseError);

            //    createBookingExcelDto.CreateBookingExcelWarehouses.Where(x => x.WarehouseCode == warehouse.WarehouseCode).ToList().ForEach(delegate (CreateBookingExcelWarehouse bookWhse)
            //    {
            //        if(bookWhse.ImportResult == "")
            //        {
            //            bookWhse.ImportResult = "Error because some data is incorrect, Please check data and upload again. พบข้อมูลบางส่วนไม่ถูกต้อง กรุณาตรวจสอบข้อมูลและทำการนำเข้าไฟล์อีกครั้ง ";
            //        }
            //    });
            //}

            //createBookingExcelDto.CreateBookingExcelPos.Where(x => x.ImportResult == "").ToList().ForEach(delegate (CreateBookingExcelPo bookPo)
            //{
            //    if (bookPo.ImportResult == "")
            //    {
            //        bookPo.ImportResult = "Error because some data is incorrect, Please check data and upload again. พบข้อมูลบางส่วนไม่ถูกต้อง กรุณาตรวจสอบข้อมูลและทำการนำเข้าไฟล์อีกครั้ง";
            //    }
            //});


            return createBookingExcelDto;
        }

        private void CheckWarehouse(ref CreateBookingExcelDto createBookingExcelDto,List<PoList> poLists,List<Warehouse> whseLists,ref List<string> listWhse,List<EstTime> estTimeList,List<TruckMaster> truckList)
        {
            var trucks = truckList;
            var estTimes = estTimeList;
            int supId = createBookingExcelDto.InternalSupGroupId;

            //foreach (var item in createBookingExcelDto.CreateBookingExcelWarehouses)
            //{
            //    var whse = whseLists.FirstOrDefault(x => x.WarehouseCode == item.WarehouseCode);
            //    if (whse == null)
            //    {
            //        item.ImportResult = "Warehouse Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขคลังสินค้านี้, กรุณาติดต่อ Data entry";
            //        listWhse.Remove(item.WarehouseCode);
            //    }
            //    else
            //    {
            //        bool truckError = false;
            //        // 4W
            //        var truck = trucks.FirstOrDefault(x => x.TruckCode == "4 W");

            //        var estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //        if (estTime == null && item.FourwheelQty > 0)
            //        {
            //            item.ImportResult = "Estimate time not set";
            //            truckError = true;
            //        }
                    
            //        // 6W
            //        truck = trucks.FirstOrDefault(x => x.TruckCode == "6 W");

            //        estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //        if (estTime == null && item.SixwheelQty > 0)
            //        {
            //            item.ImportResult = "Estimate time not set";
            //            truckError = true;
            //        }
                    
            //        // 10W
            //        truck = trucks.FirstOrDefault(x => x.TruckCode == "10 W");

            //        estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //        if (estTime == null && item.TenwheelQty > 0)
            //        {
            //            item.ImportResult = "Estimate time not set";
            //            truckError = true;
            //        }                    

            //        // 18W
            //        truck = trucks.FirstOrDefault(x => x.TruckCode == "18 W");

            //        estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //        if (estTime == null && item.EighteenwheelQty > 0)
            //        {
            //            item.ImportResult = "Estimate time not set";
            //            truckError = true;
            //        }
                   
            //        if(truckError)
            //        {
            //            listWhse.Remove(item.WarehouseCode);
            //        }
                    
            //    }
            //}
        }

        private List<BookingHeaderDto> CreateBookingDto(ref CreateBookingExcelDto createBookingExcelDto, List<PoList> poLists, List<Warehouse> whses, ref List<string> listWhse, List<EstTime> estTimes, List<TruckMaster> trucks)
        {
            List<BookingHeaderDto> bookingHeaderDtos = new List<BookingHeaderDto>();
            BookingHeaderDto bookingHeaderDto = new BookingHeaderDto();

            
            foreach (var whse in listWhse)
            {
                var bookingStart = createBookingExcelDto.BookingDate.ToLocalTime().Date;
                var bookingEnd = createBookingExcelDto.BookingDate.ToLocalTime().Date;

                var whseCode = whses.FirstOrDefault(x => x.WarehouseCode == whse);

                if(whseCode.WarehouseLevel == "W")
                {
                    CreateBookingExcelWarehouse excelWhse = new CreateBookingExcelWarehouse();
                    int estTimeMinute = 0;
                    int supId = createBookingExcelDto.InternalSupGroupId;

                    excelWhse = createBookingExcelDto.CreateBookingExcelWarehouses.FirstOrDefault(x => x.WarehouseCode == whse);
                    
                    CalculateEstTime(estTimes, trucks, excelWhse, ref estTimeMinute, supId);

                    if (excelWhse.TimeSlot.Value.ToLocalTime().Hour > 21)
                    {
                        bookingStart = bookingStart.AddDays(-1);
                    }

                    bookingStart = bookingStart.AddHours(excelWhse.TimeSlot.Value.ToLocalTime().Hour).AddMinutes(excelWhse.TimeSlot.Value.ToLocalTime().Minute);
                    bookingEnd = bookingStart.AddMinutes(estTimeMinute);

                    bookingHeaderDto = new BookingHeaderDto();
                    bookingHeaderDto.WarehouseCode = whseCode.WarehouseCode;
                    bookingHeaderDto.InternalSupGroupId = createBookingExcelDto.InternalSupGroupId;
                    bookingHeaderDto.CompanyCode = whseCode.CompanyCode;
                    bookingHeaderDto.FirstBookingStart = bookingStart;
                    bookingHeaderDto.FirstBookingEnd = bookingEnd;
                    bookingHeaderDto.BookingStart = bookingStart;
                    bookingHeaderDto.BookingEnd = bookingEnd;
                    

                    // check booking update

                    var bookingDate = createBookingExcelDto.BookingDate.ToLocalTime().Date;
                    var sup = createBookingExcelDto.SupplierCode;

                    var bookingHdrInfos = unitOfWork.BookingHeaderRepository.GetBookingHeaderBySupplierAndBookingDate(sup, bookingDate).Where(x => x.WarehouseCode == whse).ToList();

                    if (bookingHdrInfos.Count > 0)
                    {
                        // check time slot
                        foreach (var bookHdr in bookingHdrInfos)
                        {
                            if (bookHdr.BookingStart.Value.Hour == bookingStart.Hour && bookHdr.BookingStart.Value.Minute == bookingStart.Minute)
                            {
                                bookingHeaderDto.BookingId = bookHdr.BookingId;
                            }
                        }
                    }

                    bookingHeaderDto.BookingDetails = new List<BookingDetail>();

                    poLists = poLists.Distinct().ToList();

                    // add PO to bookingheader
                    foreach (var po in poLists.Where(x => x.Warehouse_Code == whseCode.WarehouseWms))
                    {
                        BookingDetail bookingDetail = new BookingDetail();
                        bookingDetail.PoNbr = po.Po_Nbr;
                        bookingDetail.Con = po.Con;
                        bookingDetail.Non = po.Non;
                        bookingDetail.FullPl = po.Full;
                        bookingDetail.CubeCon = po.Cube_Con;
                        bookingDetail.CubeNon = po.Cube_Non;
                        bookingDetail.CubeFull = po.Cube_Full;
                        bookingDetail.Weight = po.Weight;
                        bookingDetail.MerchType = po.Merch_Type;
                        bookingDetail.PlanRec = po.Plan_Receive_Date;
                        bookingDetail.Postponed = po.PostPoned;
                        bookingDetail.TotalQty = po.Total_Qty;
                        
                        bookingHeaderDto.BookingDetails.Add(bookingDetail);
                    }

                    bookingHeaderDto.MerchType = bookingHeaderDto.BookingDetails.Select(x => x.MerchType).First();
                    bookingHeaderDtos.Add(bookingHeaderDto);
                }

                if(whseCode.WarehouseLevel == "C")
                {
                    var whseCodes = whses.Where(x => x.WarehouseCode == whse);

                    foreach (var whCom in whseCodes)
                    {
                        // get po
                        var pos = poLists.Where(x => x.Warehouse_Code == whCom.WarehouseWms && x.Company_Code == whCom.CompanyCode).ToList();
                        if(pos.Count > 0)
                        {
                            CreateBookingExcelWarehouse excelWhse = new CreateBookingExcelWarehouse();
                            int estTimeMinute = 0;
                            int supId = createBookingExcelDto.InternalSupGroupId;

                            excelWhse = createBookingExcelDto.CreateBookingExcelWarehouses.FirstOrDefault(x => x.WarehouseCode == whCom.WarehouseCode);

                            CalculateEstTime(estTimes, trucks, excelWhse, ref estTimeMinute, supId);

                            if (excelWhse.TimeSlot.Value.ToLocalTime().Hour > 21)
                            {
                                bookingStart = bookingStart.AddDays(-1);
                            }

                            bookingStart = bookingStart.AddHours(excelWhse.TimeSlot.Value.ToLocalTime().Hour).AddMinutes(excelWhse.TimeSlot.Value.ToLocalTime().Minute);
                            bookingEnd = bookingStart.AddMinutes(estTimeMinute);

                            bookingHeaderDto = new BookingHeaderDto();
                            bookingHeaderDto.WarehouseCode = whCom.WarehouseCode;
                            bookingHeaderDto.InternalSupGroupId = createBookingExcelDto.InternalSupGroupId;
                            bookingHeaderDto.CompanyCode = whCom.CompanyCode;
                            bookingHeaderDto.FirstBookingStart = bookingStart;
                            bookingHeaderDto.FirstBookingEnd = bookingEnd;
                            bookingHeaderDto.BookingStart = bookingStart;
                            bookingHeaderDto.BookingEnd = bookingEnd;

                            // check booking update

                            var bookingDate = createBookingExcelDto.BookingDate.ToLocalTime().Date;
                            var sup = createBookingExcelDto.SupplierCode;

                            var bookingHdrInfos = unitOfWork.BookingHeaderRepository.GetBookingHeaderBySupplierAndBookingDate(sup, bookingDate)
                                .Where(x => x.WarehouseCode == whCom.WarehouseCode && x.CompanyCode == whCom.CompanyCode).ToList();

                            if (bookingHdrInfos.Count > 0)
                            {
                                // check time slot
                                foreach (var bookHdr in bookingHdrInfos)
                                {
                                    if (bookHdr.BookingStart.Value.Hour == bookingStart.Hour && bookHdr.BookingStart.Value.Minute == bookingStart.Minute)
                                    {
                                        bookingHeaderDto.BookingId = bookHdr.BookingId;
                                    }
                                }
                            }

                            bookingHeaderDto.BookingDetails = new List<BookingDetail>();

                            poLists = poLists.Distinct().ToList();

                            // add PO to bookingheader
                            foreach (var po in poLists.Where(x => x.Warehouse_Code == whCom.WarehouseWms && x.Company_Code == whCom.CompanyCode))
                            {
                                BookingDetail bookingDetail = new BookingDetail();
                                bookingDetail.PoNbr = po.Po_Nbr;
                                bookingDetail.Con = po.Con;
                                bookingDetail.Non = po.Non;
                                bookingDetail.FullPl = po.Full;
                                bookingDetail.CubeCon = po.Cube_Con;
                                bookingDetail.CubeNon = po.Cube_Non;
                                bookingDetail.CubeFull = po.Cube_Full;
                                bookingDetail.Weight = po.Weight;
                                bookingDetail.MerchType = po.Merch_Type;
                                bookingDetail.PlanRec = po.Plan_Receive_Date;
                                bookingDetail.Postponed = po.PostPoned;
                                bookingDetail.TotalQty = po.Total_Qty;

                                bookingHeaderDto.BookingDetails.Add(bookingDetail);
                            }

                            bookingHeaderDto.MerchType = bookingHeaderDto.BookingDetails.Select(x => x.MerchType).First();
                            bookingHeaderDtos.Add(bookingHeaderDto);
                        }
                    }

                }

            }

            return bookingHeaderDtos;

        }

        private static void CalculateEstTime(List<EstTime> estTimes, List<TruckMaster> trucks, CreateBookingExcelWarehouse excelWhse, ref int estTimeMinute, int supId)
        {
            //var truck = trucks.FirstOrDefault(x => x.TruckCode == "4 W");

            //var estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //if (excelWhse.FourwheelQty > 0)
            //{
            //    estTimeMinute += ((estTime.HourEst.Value * 60) + (estTime.MinEst.Value)) * excelWhse.FourwheelQty;
            //}

            //// 6W
            //truck = trucks.FirstOrDefault(x => x.TruckCode == "6 W");

            //estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //if (excelWhse.SixwheelQty > 0)
            //{
            //    estTimeMinute += ((estTime.HourEst.Value * 60) + (estTime.MinEst.Value)) * excelWhse.SixwheelQty;
            //}

            //// 10W
            //truck = trucks.FirstOrDefault(x => x.TruckCode == "10 W");

            //estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //if (excelWhse.TenwheelQty > 0)
            //{
            //    estTimeMinute += ((estTime.HourEst.Value * 60) + (estTime.MinEst.Value)) * excelWhse.TenwheelQty;
            //}

            //// 18W
            //truck = trucks.FirstOrDefault(x => x.TruckCode == "18 W");

            //estTime = estTimes.FirstOrDefault(x => x.InternalSupGroupId == supId && x.InternalTruckId == truck.InternalTruckId);

            //if (excelWhse.EighteenwheelQty > 0)
            //{
            //    estTimeMinute += ((estTime.HourEst.Value * 60) + (estTime.MinEst.Value)) * excelWhse.EighteenwheelQty;
            //}

            
        }
    }
}
