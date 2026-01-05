using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;

namespace Makro.IMS.POServices.Api.Services
{
    public class PoService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;

        public PoService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public async Task<List<PoList>> GetPoListBySupCodeAndCompany(string supCode, string companyCode, DateTime bookingDate)
        {
            var pos = unitOfWork.PoListRepository.GetPoNotBooking(supCode, companyCode).ToList();
            var poResult = new List<PoList>();

            foreach (var po in pos.OrderBy(x=>x.Warehouse_Code).ThenBy(x=>x.Po_Nbr))
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

                if (po.PostPoned.ToUpper() == "N")
                {
                    if (po.Plan_Receive_Date.Date != bookingDate.Date)
                    {
                        //throw new Exception("PO do not postpone" + "\r\n" + "หมายเลขใบสั่งซื้อไม่สามารถเลื่อนวันส่งสินค้าได้");
                    }
                    else
                    {
                        poResult.Add(po);
                    }
                }
                else
                {
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

            warehouse = warehouseList.FirstOrDefault(x => x.WarehouseMain == poResult.Warehouse_Code && x.CompanyCode == poResult.Company_Code);

            poResult.Warehouse_Code = warehouse.WarehouseCode;
            poResult.Company_Code = warehouse.CompanyCode;

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


            if (poResult.Booking_Id != null)
            {
                throw new Exception("PO already booked" + "\r\n" + "หมายเลขใบสั่งซื้อถูกนัดหมายเรียบร้อยแล้ว");
                
            }

            if (poResult.Plan_Receive_Date <= DateTime.Now.AddDays(-7))
            {
                //throw new Exception("PO over 7 days");
                throw new Exception("Plan received date and booking date are different more than 7 days. Please change booking date" + "\r\n" + "ไม่สามารถนัดหมายได้ เนื่องจากวันนัดหมายกับวันในใบสั่งซื้อไม่ตรงกันเกิน 7 วัน กรุณาเปลี่ยนวันที่นัดหมาย");
            }

            if((poResult.Plan_Receive_Date.Date - bookingDate.Date).TotalDays < -7)
            {
                throw new Exception("Plan received date and booking date are different more than 7 days. Please change booking date" + "\r\n" + "ไม่สามารถนัดหมายได้ เนื่องจากวันนัดหมายกับวันในใบสั่งซื้อไม่ตรงกันเกิน 7 วัน กรุณาเปลี่ยนวันที่นัดหมาย");
            }

            if(poResult.PostPoned.ToUpper() == "N")
            {
                if (poResult.Plan_Receive_Date.Date != bookingDate.Date)
                {
                    throw new Exception("PO do not postpone" + "\r\n" + "หมายเลขใบสั่งซื้อไม่สามารถเลื่อนวันส่งสินค้าได้");
                }
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

            if (poResult.PostPoned.ToUpper() == "N")
            {
                if (poResult.Plan_Receive_Date.Date != DateTime.Now.Date)
                {
                    throw new Exception("PO do not postpone");
                }
            }

            //Supplier supplier = unitOfWork.SupplierRepository.GetSupplierBySupCode(supCode);

            //if (supplier.SupName.Contains("DO NOT POSTPONE"))
            //{
            //    if (poResult.Plan_Receive_Date.Date != DateTime.Now.Date)
            //    {
            //        throw new Exception("Supplier do not postpone");
            //    }
            //}


            return poResult;

        }


        #region +++ CDC +++

        public async Task<List<PoList>> GetPoListBySupCodeAndCompanyCDC(string supCode, string companyCode, DateTime bookingDate)
        {
            var pos = unitOfWork.PoListRepository.GetPoNotBookingCDC(supCode, companyCode).ToList();
            var poResult = new List<PoList>();

            foreach (var po in pos.OrderBy(x => x.Warehouse_Code).ThenBy(x => x.Po_Nbr))
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

                if (po.PostPoned.ToUpper() == "N")
                {
                    if (po.Plan_Receive_Date.Date != bookingDate.Date)
                    {
                        //throw new Exception("PO do not postpone" + "\r\n" + "หมายเลขใบสั่งซื้อไม่สามารถเลื่อนวันส่งสินค้าได้");
                    }
                    else
                    {
                        poResult.Add(po);
                    }
                }
                else
                {
                    poResult.Add(po);
                }

            }


            return poResult;
        }


        public async Task<PoList> GetPoListByPoCDC(string poNo, string supCode, string companyCode, DateTime bookingDate)
        {
            PoList poResult;

            //List<Warehouse> warehouseList = unitOfWork.WarehouseRepository.GetWarehouses().ToList();
            //Warehouse warehouse;

            poResult = unitOfWork.PoListRepository.GetPoByPoNoCDC(poNo, supCode, companyCode);

            if (poResult == null)
            {
                throw new Exception("PO Not found, Please contact data entry" + "\r\n" + "ไม่พบหมายเลขใบสั่งซื้อนี้, กรุณาติดต่อ Data entry");
            }

            //warehouse = warehouseList.FirstOrDefault(x => x.WarehouseMain == poResult.Warehouse_Code && x.CompanyCode == poResult.Company_Code);

            //poResult.Warehouse_Code = warehouse.WarehouseCode;
            //poResult.Company_Code = warehouse.CompanyCode;

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


            if (poResult.Booking_Id != null)
            {
                throw new Exception("PO already booked" + "\r\n" + "หมายเลขใบสั่งซื้อถูกนัดหมายเรียบร้อยแล้ว");

            }

            if (poResult.Plan_Receive_Date <= DateTime.Now.AddDays(-7))
            {
                //throw new Exception("PO over 7 days");
                throw new Exception("Plan received date and booking date are different more than 7 days. Please change booking date" + "\r\n" + "ไม่สามารถนัดหมายได้ เนื่องจากวันนัดหมายกับวันในใบสั่งซื้อไม่ตรงกันเกิน 7 วัน กรุณาเปลี่ยนวันที่นัดหมาย");
            }

            if ((poResult.Plan_Receive_Date.Date - bookingDate.Date).TotalDays < -7)
            {
                throw new Exception("Plan received date and booking date are different more than 7 days. Please change booking date" + "\r\n" + "ไม่สามารถนัดหมายได้ เนื่องจากวันนัดหมายกับวันในใบสั่งซื้อไม่ตรงกันเกิน 7 วัน กรุณาเปลี่ยนวันที่นัดหมาย");
            }

            if (poResult.PostPoned.ToUpper() == "N")
            {
                if (poResult.Plan_Receive_Date.Date != bookingDate.Date)
                {
                    throw new Exception("PO do not postpone" + "\r\n" + "หมายเลขใบสั่งซื้อไม่สามารถเลื่อนวันส่งสินค้าได้");
                }
            }
            return poResult;

        }


        #endregion
    }
}
