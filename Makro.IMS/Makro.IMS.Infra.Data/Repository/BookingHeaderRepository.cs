using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class BookingHeaderRepository : IBookingHeaderRepository
    {
        private IMSContext context;
        private DbSet<BookingHeader> dbSet;

        public BookingHeaderRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingHeaders;
        }

        public IEnumerable<BookingHeader> GetBookingHeaders()
        {
            return this.dbSet.ToList();
        }

        public BookingHeader? GetBookingHeaderById(int keyId)
        {
            return this.dbSet.Find(keyId)!;
        }

        public IEnumerable<BookingHeader>? GetDashboard(string warehouseCode)
        {

            return this.dbSet.Where(x => x.BookingStart > DateTime.Now.AddDays(-6) && x.BookingStart < DateTime.Now.AddDays(30) && x.WarehouseCode == warehouseCode).ToList();                        

        }

        public IEnumerable<BookingHeader>? GetBookingHeaderBySupplierAndBookingDate(string supCode, DateTime bookingDate)
        {
            var result = new List<BookingHeader>();

            var keyId = this.dbSet.Where(x => x.SupCode == supCode && x.BookingStart.Value.Date == bookingDate)
                        .Select(x => x.InternalKeyId).FirstOrDefault();

            keyId = keyId ?? 0;

            result = this.dbSet.Where(x => x.InternalKeyId == keyId).ToList();

            return result;
        }

        public IEnumerable<BookingHeader>? GetBookingHeaderByKeyId(int keyId)
        {
            var result = new List<BookingHeader>();

            result = this.dbSet.Where(x => x.InternalKeyId.Value == keyId).ToList();

            return result;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Add(BookingHeader bookingHeader)
        {
            bool result = true;

            this.dbSet.Add(bookingHeader);

            return result;
        }

        public bool Remove(BookingHeader bookingHeader)
        {
            bool result = true;

            this.dbSet.Remove(bookingHeader);

            return result;
        }

        public bool Update(BookingHeader bookingHeader)
        {
            bool result = true;

            this.dbSet.Update(bookingHeader);

            return result;
        }

        public IQueryable<BookingHeader> GetBookingHeadersPaged(int internalSubGroup)
        {
            return this.dbSet.Where(x=>x.InternalSupGroupId == internalSubGroup && x.BookingStart > DateTime.Now.AddDays(-60) && x.BookingStart < DateTime.Now.AddDays(30)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetBookingHeadersPaged(List<string> whseList, List<int> supGroupList)
        {
            return this.dbSet.Where(x => supGroupList.Contains(x.InternalSupGroupId.Value) && whseList.Contains(x.WarehouseCode) 
            && x.BookingStart > DateTime.Now.AddDays(-60) && x.BookingStart < DateTime.Now.AddDays(30)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetBookingHeadersPaged(List<string> whseList)
        {
            return this.dbSet.Where(x => whseList.Contains(x.WarehouseCode)
            && x.BookingStart > DateTime.Now.AddDays(-60) && x.BookingStart < DateTime.Now.AddDays(30)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetBookingHeadersPaged(List<int> supGroupList)
        {
            return this.dbSet.Where(x => supGroupList.Contains(x.InternalSupGroupId.Value) 
            && x.BookingStart > DateTime.Now.AddDays(-60) && x.BookingStart < DateTime.Now.AddDays(30)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetBookingHeadersPaged()
        {
            return this.dbSet.Where(x=>x.BookingStart > DateTime.Now.AddDays(-60) && x.BookingStart < DateTime.Now.AddDays(30)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInPaged()
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "APPROVED").AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInPaged(List<string> whseList, List<int> supGroupList)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "APPROVED" && supGroupList.Contains(x.InternalSupGroupId.Value) && whseList.Contains(x.WarehouseCode)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInPaged(List<string> whseList)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "APPROVED" && whseList.Contains(x.WarehouseCode)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInPaged(List<int> supGroupList)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "APPROVED" && supGroupList.Contains(x.InternalSupGroupId.Value)).AsNoTracking();
        }


        public IQueryable<BookingHeader> GetPreCheckInCompletedPaged()
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "INTRANSIT").AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInCompletedPaged(List<string> whseList, List<int> supGroupList)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "INTRANSIT" && supGroupList.Contains(x.InternalSupGroupId.Value) && whseList.Contains(x.WarehouseCode)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInCompletedPaged(List<string> whseList)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "INTRANSIT" && whseList.Contains(x.WarehouseCode)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInCompletedPaged(List<int> supGroupList)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "INTRANSIT" && supGroupList.Contains(x.InternalSupGroupId.Value)).AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInCompletedPaged(int internalSubGroup)
        {
            return this.dbSet.Where(x => x.InternalSupGroupId == internalSubGroup && x.Status.ToUpper() == "INTRANSIT").AsNoTracking();
        }

        public IQueryable<BookingHeader> GetPreCheckInPaged(int internalSubGroup)
        {
            return this.dbSet.Where(x =>x.InternalSupGroupId == internalSubGroup && x.Status.ToUpper() == "APPROVED").AsNoTracking();
        }

        public BookingHeader? GetBookingHeaderByDoorAndSlotTime(int internalDoorId, DateTime startDate, DateTime endDate)
        { 
            return this.dbSet.Where(x => x.InternalDoorId == internalDoorId && 
            (
                (startDate >= x.BookingStart && startDate <= x.BookingEnd) || 
                (endDate >= x.BookingStart && endDate <= x.BookingEnd) ||
                (x.BookingStart >= startDate && x.BookingEnd <= endDate)
            )
            ).FirstOrDefault();
        }

        public BookingHeader GetBookingHeadersByBookingId(string bookingId)
        {
            return this.dbSet.Where(x => x.BookingId == bookingId).FirstOrDefault();
        }

        public IEnumerable<BookingHeader>? GetBookingHeaderForCheckIn(string warehouseCode)
        {
            return this.dbSet.Where(x => x.Status.ToUpper() == "INTRANSIT");
        }

        public bool UpdateStatus(int id, string status)
        {
            throw new NotImplementedException();
        }

        public List<QueueManageDto> GetBookingQueue(string criteria)
        {
            string sqlCmd = "select bh.internal_header_key as internalHeaderkey,btc.internal_truck_check_in_id as internalTruckCheckInId,booking_id as bookingId,warehouse_code as warehouseCode,company_code as companyCode,"
            + "sup_code as supCode,sup_name as supName,merch_type as operationType,booking_start as bookingStart, booking_end as bookingEnd,btc.license_plate as licensePlate,nvl(btc.license_plate_2,' ') as licensePlate2,"
            + "tm.truck_code as truck,btc.tel_no as telNo,btc.queue_seq as QueueSeq,btc.arrived_time as arrivedTime,btc.ondock_time as onDockTime,"
            + "nvl(round((nvl(btc.ondock_time, sysdate) - btc.assign_queue_time) * 24 * 60 * 60, 0),0) as waitingTime,btc.status "
            + "btc.Waiting_Document as WaitingDocument,btc.Remark,btc.Waiting_Document_Time as WaitingDocumentTime, (select count(*) from booking_truck_check_in t where t.internal_header_key = bh.internal_header_key) as TotalTruck, "
            + "btc.date_time_stamp as lastUpDate, case when bh.back_haul = 1 then 'BH' else 'Direct' end as BackHaul  "
            + "from booking_header bh "
            + "left join booking_truck_check_in btc on bh.internal_header_key = btc.internal_header_key "
            + "left join truck_master tm on btc.internal_truck_id = tm.internal_truck_id "
            + "where btc.status IN ('CHECKIN','QUEUE','CALLTRUCK','ONDOCK','UNLOADING','UNLOADED','LEAVEDOOR','WAITINGDOC','SUBMITDOC','CHECKOUT') ";
            
            if (!string.IsNullOrEmpty(criteria))
            {
                sqlCmd += criteria;
            }

            sqlCmd += " order by btc.assign_queue_time,btc.queue_seq  ";

            var result = this.context.Set<QueueManageDto>()
                        .FromSqlRaw(sqlCmd)
                        .ToList();

            return result;
        }

        public List<QueueManageDto> GetBookingQueuePerTruck(string criteria)
        {
            string sqlCmd = "select bh.internal_header_key as internalHeaderkey,btc.internal_truck_check_in_id as internalTruckCheckInId,booking_id as bookingId,warehouse_code as warehouseCode,company_code as companyCode,"
            + "sup_code as supCode,sup_name as supName,merch_type as operationType,booking_start as bookingStart, booking_end as bookingEnd,btc.license_plate as licensePlate,nvl(btc.license_plate_2,' ') as licensePlate2,"
            + "tm.truck_code as truck,btc.tel_no as telNo,btc.queue_seq as QueueSeq,btc.arrived_time as arrivedTime,btc.ondock_time as onDockTime,"
            + "nvl(round((nvl(btc.ondock_time, sysdate) - btc.assign_queue_time) * 24 * 60 * 60, 0),0) as WaitingTime,btc.status,"
            + "btc.Waiting_Document as WaitingDocument,btc.Remark,btc.Waiting_Document_Time as WaitingDocumentTime, (select count(*) from booking_truck_check_in t where t.internal_header_key = bh.internal_header_key) as TotalTruck,  "
            + "btc.date_time_stamp as lastUpDate, case when bh.back_haul = 1 then 'BH' else 'Direct' end as BackHaul "
            + "from booking_header bh "
            + "left join booking_truck_check_in btc on bh.internal_header_key = btc.internal_header_key "
            + "left join truck_master tm on btc.internal_truck_id = tm.internal_truck_id "
            + "where btc.status IN ('CHECKIN','QUEUE','CALLTRUCK','ONDOCK','UNLOADING','UNLOADED','LEAVEDOOR','WAITINGDOC','SUBMITDOC','CHECKOUT') ";
            
            if (!string.IsNullOrEmpty(criteria))
            {
                sqlCmd += criteria;
            }

            sqlCmd += " order by btc.assign_queue_time,btc.queue_seq  ";

            var result = this.context.Set<QueueManageDto>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

        public List<QueueManageDto> GetGatePassByTelNo(string telNo)
        {
            string sqlCmd = "select bh.internal_header_key as internalHeaderkey,btc.internal_truck_check_in_id as internalTruckCheckInId,booking_id as bookingId,warehouse_code as warehouseCode,company_code as companyCode,"
            + "sup_code as supCode,sup_name as supName,merch_type as operationType,booking_start as bookingStart, booking_end as bookingEnd,btc.license_plate as licensePlate,nvl(btc.license_plate_2,' ') as licensePlate2,"
            + "tm.truck_code as truck,btc.tel_no as telNo,btc.queue_seq as QueueSeq,btc.arrived_time as arrivedTime,btc.ondock_time as onDockTime,"
            + "nvl(round((nvl(btc.ondock_time, sysdate) - btc.assign_queue_time) * 24 * 60 * 60, 0),0) as WaitingTime,btc.status,"
            + "btc.Waiting_Document as WaitingDocument,btc.Remark,btc.Waiting_Document_Time as WaitingDocumentTime, (select count(*) from booking_truck_check_in t where t.internal_header_key = bh.internal_header_key) as TotalTruck, "
            + "btc.date_time_stamp as lastUpDate, case when bh.back_haul = 1 then 'BH' else 'Direct' end as BackHaul  "
            + "from booking_header bh "
            + "left join booking_truck_check_in btc on bh.internal_header_key = btc.internal_header_key "
            + "left join truck_master tm on btc.internal_truck_id = tm.internal_truck_id "
            + "where btc.status IN ('INTRANSIT','CHECKIN','QUEUE','CALLTRUCK','ONDOCK','UNLOADING','UNLOADED','LEAVEDOOR','SUBMITDOC') "
            + " and btc.tel_no = '" + telNo + "' ";

            var result = this.context.Set<QueueManageDto>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

        public List<TruckCheckIn> GetGatePassByLicensePlate(string licensePlate)
        {
            string sqlCmd = "select bk.booking_date as BookingDate,bh.booking_id as BookingId,replace(bt.license_plate,'-','') as LicensePlate,"
            + "bt.driver_name as DriverName,bt.tel_no as TelNo,bt.line_id as LineId,tm.truck_code as TruckType,"
            + "bh.booking_start - wh.advance_checkin_time/1440 as BookingStart,"
            + "bh.booking_start + wh.late_checkin_time/1440 as BookingEnd,"
            //+ "bh.booking_end as BookingEnd,"
            + "bh.warehouse_code as WarehouseCode,"
            + "bh.company_code as CompanyCode,bh.sup_code as SupCode,bh.sup_name as SupName,"
            + "bh.merch_type as operationType,bt.status "
            + "from booking_truck_check_in bt "
            + "inner join booking_header bh on bh.internal_header_key = bt.internal_header_key "
            + "inner join truck_master tm on bt.internal_truck_id = tm.internal_truck_id "
            + "inner join booking_key bk on bh.internal_key_id = bk.internal_key_id "
            + "inner join warehouse wh on bh.warehouse_code = wh.warehouse_code "
            + "where (replace(license_plate,'-','') = '" + licensePlate + "' " 
            + " or  replace(license_plate_2,'-','') = '" + licensePlate + "') "  
            + " and bt.status = 'INTRANSIT' and trunc(bk.booking_date) = trunc(sysdate)";

            var result = this.context.Set<TruckCheckIn>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

        public List<TruckCheckIn> GetTruckCheckoutByLicensePlate(string licensePlate)
        {
            string sqlCmd = "select bk.booking_date as BookingDate,bh.booking_id as BookingId,replace(bt.license_plate,'-','') as LicensePlate,"
            + "bt.driver_name as DriverName,bt.tel_no as TelNo,bt.line_id as LineId,tm.truck_code as TruckType,"
            + "bh.booking_start - wh.advance_checkin_time/1440 as BookingStart,"
            + "bh.booking_start + wh.late_checkin_time/1440 as BookingEnd,"
            //+ "bh.booking_end as BookingEnd,"
            + "bh.warehouse_code as WarehouseCode,"
            + "bh.company_code as CompanyCode,bh.sup_code as SupCode,bh.sup_name as SupName,"
            + "bh.merch_type as operationType,bt.status "
            + "from booking_truck_check_in bt "
            + "inner join booking_header bh on bh.internal_header_key = bt.internal_header_key "
            + "inner join truck_master tm on bt.internal_truck_id = tm.internal_truck_id "
            + "inner join booking_key bk on bh.internal_key_id = bk.internal_key_id "
            + "inner join warehouse wh on bh.warehouse_code = wh.warehouse_code "
            + "where (replace(license_plate,'-','') = '" + licensePlate + "' "
            + " or  replace(license_plate_2,'-','') = '" + licensePlate + "') "
            + "and bt.status <> 'INTRANSIT' and bt.status <> 'CHECKOUT'";// and trunc(bk.booking_date) = trunc(sysdate)";

            var result = this.context.Set<TruckCheckIn>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

    }
}
