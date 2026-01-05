using AutoMapper;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;

namespace Makro.IMS.Services.Api.Services
{
    public class PoService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveIListProcessor _sieveProcessor;
        
        public PoService(
            ISieveIListProcessor sieveProcessor)
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<PoList>> GetPoListBySupCodeAndCompany(string supCode, string companyCode, DateTime bookingDate)
        {
            var pos = unitOfWork.PoListRepository.GetPoNotBooking(supCode, companyCode).ToList();

            // remove po not in range by warehouse
            var whses = unitOfWork.WarehouseRepository.GetWarehouses();

            var poResult = new List<PoList>();

            foreach (var wh in whses)
            {
                foreach (var po in pos.Where(x=>x.Warehouse_Code == wh.WarehouseWms 
                    && x.Company_Code == wh.CompanyCode
                    && x.Plan_Receive_Date >= bookingDate.AddDays((wh.PoBeforePeriod.Value - 1) * -1)
                    && x.Plan_Receive_Date <= bookingDate.AddDays(wh.PoAfterPeriod.Value - 1)
                    && x.Expire_Date.Value.Date > bookingDate.Date)
                    //&& x.Plan_Receive_Date < x.Expire_Date)
                    .OrderBy(x=>x.Booking_Id)
                    .ThenBy(x => x.Warehouse_Code).ThenBy(x => x.Plan_Receive_Date)
                    .ThenBy(x=>x.Merch_Type).ThenBy(x=>x.Po_Nbr)
                    )
                {
                    if (!po.Con.HasValue)
                        po.Con = 0;

                    if (!po.Non.HasValue)
                        po.Non = 0;

                    if (!po.Full.HasValue)
                        po.Full = 0;

                    if (!po.Cube_Con.HasValue)
                        po.Cube_Con = 0;

                    if (!po.Cube_Non.HasValue)
                        po.Cube_Non = 0;

                    if (!po.Cube_Full.HasValue)
                        po.Cube_Full = 0;

                    poResult.Add(po);

                }
            }

            return poResult;
        }

        public async Task<List<PoList>> GetPoListByPos(string pos,string supCode, string companyCode)
        {

            return unitOfWork.PoListRepository.GetPoByPos(pos,supCode, companyCode).ToList();
        }

        public async Task<PoList> GetPoListByPo(string poNo, string supCode, string companyCode,DateTime bookingDate)
        {
            PoList poResult;

            List<Warehouse> warehouseList = unitOfWork.WarehouseRepository.GetWarehouses().ToList();
            Warehouse warehouse;

            poResult = unitOfWork.PoListRepository.GetPoByPoNo(poNo, supCode, companyCode);

            if (poResult == null)
            {
                throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
            }

            warehouse = warehouseList.FirstOrDefault(x => x.WarehouseWms == poResult.Warehouse_Code && x.CompanyCode == poResult.Company_Code);

            if(warehouse != null)
            {
                poResult.Warehouse_Code = warehouse.WarehouseCode;
                poResult.Company_Code = warehouse.CompanyCode;
            }
            else
            {
                throw new Exception("Warehouse Not found, Please contact data entry" + "\r\n" + "ไม่พบข้อมูล warehouse, กรุณาติดต่อ Data entry");
            }


            if (!poResult.Con.HasValue)
                poResult.Con = 0;

            if (!poResult.Non.HasValue)
                poResult.Non = 0;

            if (!poResult.Full.HasValue)
                poResult.Full = 0;

            if (!poResult.Cube_Con.HasValue)
                poResult.Cube_Con = 0;

            if (!poResult.Cube_Non.HasValue)
                poResult.Cube_Non = 0;

            if (!poResult.Cube_Full.HasValue)
                poResult.Cube_Full = 0;

            // remove check po already booked.
            //if (poResult.Booking_Id != null)
            //{
            //    throw new Exception("PO already booked" + "\r\n" + "หมายเลขใบสั่งซื้อถูกนัดหมายเรียบร้อยแล้ว");
                
            //}

            if(poResult.Expire_Date.Value.Date <= bookingDate.Date)
            {
                throw new Exception("PO already expired" + "\r\n" + "หมายเลขใบสั่งซื้อนี้หมดอายุแล้ว");
            }

            // compare booking date and plan receive date over range of booking date
            if(poResult.Plan_Receive_Date >= bookingDate.Date.AddDays((warehouse.PoBeforePeriod.Value - 1) * -1)
                && poResult.Plan_Receive_Date <= bookingDate.Date.AddDays(warehouse.PoAfterPeriod.Value - 1))
            {

            }
            else
            {
                throw new Exception("Plan received date and booking date not in the range can booking (" 
                    + bookingDate.Date.AddDays((warehouse.PoBeforePeriod.Value - 1) * -1).ToString("dd-MM-yyyy")
                    + " - " + bookingDate.Date.AddDays(warehouse.PoAfterPeriod.Value - 1).ToString("dd-MM-yyyy") + "). Please change booking date"
                    + "\r\n" + "ไม่สามารถนัดหมายได้ เนื่องจากวันนัดหมายกับวันในใบสั่งซื้อไม่อยู่ในช่วงที่กำหนด ("
                    + bookingDate.Date.AddDays((warehouse.PoBeforePeriod.Value - 1) * -1).ToString("dd-MM-yyyy")
                    + " - " + bookingDate.Date.AddDays(warehouse.PoAfterPeriod.Value - 1).ToString("dd-MM-yyyy") + "). กรุณาเปลี่ยนวันที่นัดหมาย");
            }

            return poResult;

        }

        public async Task<PoList> GetPoNoBooking(string poNo, string supCode, string companyCode)
        {
            PoList poResult;

            poResult = unitOfWork.PoListRepository.GetPoByPoNo(poNo, supCode, companyCode);

            if (poResult == null)
            {
                throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
            }

            if (poResult.Plan_Receive_Date <= DateTime.Now.AddDays(-7))
            {
                throw new Exception("PO over 7 days");
            }

            return poResult;

        }

        public async Task<List<PoList>> GetPoByPo(string poNo)
        {
            List<PoList> poResult;

            poResult = unitOfWork.PoListRepository.GetPoByPo(poNo);

            if (poResult == null)
            {
                throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
            }
            return poResult;

        }

        public async Task<List<PoLog>> GetPoLog(string poNo)
        {
            List<PoLog> poResult;

            poResult = unitOfWork.PoLogRepository.GetPoLogByPoNo(poNo).ToList();

            return poResult;

        }

        public async Task<bool> UpdateDcDelay(PoDcDelay poDcDelay,string userName)
        {
            unitOfWork.PoListRepository.UpdatePoPostponed(poDcDelay.PoNo, poDcDelay.DelayReason);
            unitOfWork.PoListRepository.UpdatePoPostponedInBookingDetail(poDcDelay.PoNo, poDcDelay.DelayReason);

            PoLog poLog = new PoLog();
            poLog.PoNbr = poDcDelay.PoNo;
            poLog.UserStamp = userName;
            poLog.DateTimeStamp = DateTime.Now;
            poLog.Log = poDcDelay.DelayReason;

            unitOfWork.PoLogRepository.Add(poLog);

            unitOfWork.Save();
            return true;
        }

        public async Task<PagedResult<PoMonitor>> GetPoMonitor(SieveModel sieveModel, string userId)
        {
            var queryable = unitOfWork.PoListRepository.GetPoMonitor(userId).ToList();

            return await _sieveProcessor.GetPagedIListAsync<PoMonitor>(queryable, sieveModel);
        }

        public async Task<PagedResult<PoMonitor>> GetPoMonitorExport(SieveModel sieveModel, string userId)
        {
            var queryable = unitOfWork.PoListRepository.GetPoMonitor(userId).ToList();

            return await _sieveProcessor.GetPagedExportIListAsync<PoMonitor>(queryable, sieveModel);
        }
    }
}
