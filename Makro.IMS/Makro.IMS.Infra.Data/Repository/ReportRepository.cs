using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class ReportRepository : IDisposable
    {
        private IMSContext context;        

        public ReportRepository(IMSContext context)
        {
            this.context = context;            
        }

        public List<ReportGatePass> GetGatePass(int internalHeaderKey)
        {
            string sqlCmd = "select booking_id as BookingId,w.warehouse_code as WarehouseCode,w.warehouse_wms as WarehouseWms,w.company_code as CompanyCode,w.warehouse_name as WarehouseName" +
                ",w.contact_name as ContactName,w.contact_e_mail as ContactEmail, w.phone_number as PhoneNumber,w.address1 as Address1,w.address2 as Address2,w.address3 as Address3" +
                ",w.city as City,w.zipcode as ZipCode,p.po_nbr as PoNbr,sup_code as SupCode,sup_name as SupName,contact_tel as ContactTel,booking_start as BookingStart" +
                ",b.license_plate as LicensePlate,b.driver_name as DriverName,b.tel_no as TelNo,c.truck_code as TruckCode " +
                ", (select sum(total_qty) from mt_po_list where po_nbr = p.po_nbr) as TotalQty " +
                ", (select description from operation where operation_name = a.merch_type and warehouse_code = a.warehouse_code and rownum = 1) as MerchType" +
                //", select merch_type from mt_po_list where po_nbr = p.po_nbr and rownum = 1) as MerchType " + 
                ", booking_start - (w.advance_checkin_time/(24*60)) as WarehouseArrived " +
                ", a.original_merch_type as OriginalMerchType " +
                ", (select sum(full) from mt_po_list where po_nbr = p.po_nbr) as FullPl " +
                ", (select sum(half) from mt_po_list where po_nbr = p.po_nbr) as HalfPl " +
                ", (select sum(con+non) from mt_po_list where po_nbr = p.po_nbr) as LooseQty " +
                ", (select plan_receive_date from mt_po_list where po_nbr = p.po_nbr and rownum = 1) as OrderDate " +
                ", case when (select trunc(plan_receive_date) from mt_po_list where po_nbr = p.po_nbr and rownum = 1) < trunc(a.booking_start) then 'Late' else 'ON Time' end as IsLate " +
                ", b.license_plate_2 as LicensePlate2,b.internal_truck_check_in_id as id,rownum as No " +
                ", (select delay_reason from mt_po_list where po_nbr = p.po_nbr and rownum = 1) as reason " +
                ", a.back_haul as BackHaul " +
                "from booking_header a " +
                "left join booking_truck_check_in b on a.internal_header_key = b.internal_header_key " +
                "left join booking_truck_check_in_detail p on b.internal_truck_check_in_id = p.internal_truck_check_in_id " +
                "left join truck_master c on b.internal_truck_id = c.internal_truck_id " +
                "left join warehouse w on a.warehouse_code = w.warehouse_code " +
                "where a.internal_header_key = " + internalHeaderKey.ToString() + " " +
                "order by p.po_nbr";

            var result = this.context.Set<ReportGatePass>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

        public ReportSummaryBooking? GetBookingSummary(string warehouseCode,string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;

            var result = this.context.Set<ReportSummaryBooking>()
                .FromSqlRaw("BEGIN RPT_SUMMARY_BOOKING (:V_WAREHOUSE_CODE,:V_COMPANY_CODE,:V_CURSOR);  END;", new object[] { vWarehouseCode, vCompany, vCursor })
                .ToList().FirstOrDefault();

            return result;
        }

        public List<ReportTruckStatus> GetTruckStatuses(string warehouseCode,string companyCode,DateTime fromDate,DateTime toDate)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vBookingDateFrom = new OracleParameter("V_BOOKING_DATE_FROM", OracleDbType.Date, ParameterDirection.Input);
            vBookingDateFrom.Value = fromDate;
            var vBookingDateTo = new OracleParameter("V_BOOKING_DATE_TO", OracleDbType.Date, ParameterDirection.Input);
            vBookingDateTo.Value = toDate;


            var result = this.context.Set<ReportTruckStatus>()
                .FromSqlRaw("BEGIN RPT_TRUCK_STATUS (:V_WAREHOUSE_CODE,:V_COMPANY_CODE,:V_BOOKING_DATE_FROM,:V_BOOKING_DATE_TO,:V_CURSOR);  END;", new object[] { vWarehouseCode, vCompany,vBookingDateFrom,vBookingDateTo, vCursor })
                .ToList();

            return result;
        }

        public List<ReportTransactionTrack> GetTransactionTrack(string warehouseCode, string companyCode, DateTime fromDate, DateTime toDate)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vBookingStart = new OracleParameter("V_BOOKING_START", OracleDbType.Date, ParameterDirection.Input);
            vBookingStart.Value = fromDate;
            var vBookingEnd = new OracleParameter("V_BOOKING_END", OracleDbType.Date, ParameterDirection.Input);
            vBookingEnd.Value = toDate;


            var result = this.context.Set<ReportTransactionTrack>()
                .FromSqlRaw("BEGIN RPT_TRANSACTION_TRACKING (:V_WAREHOUSE_CODE,:V_COMPANY_CODE,:V_BOOKING_START,:V_BOOKING_END,:V_CURSOR);  END;", new object[] { vWarehouseCode, vCompany,vBookingStart,vBookingEnd, vCursor })
                .ToList();

            return result;
        }

        public List<ReportSlottimeBooking> GetSlottimeBooking(string warehouseCode, string companyCode, DateTime fromDate)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vBookingDate = new OracleParameter("V_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);
            vBookingDate.Value = fromDate;            


            var result = this.context.Set<ReportSlottimeBooking>()
                .FromSqlRaw("BEGIN RPT_SLOTTIME_BOOKING (:V_WAREHOUSE_CODE,:V_COMPANY_CODE,:V_BOOKING_DATE,:V_CURSOR);  END;", new object[] { vWarehouseCode, vCompany, vBookingDate, vCursor })
                .ToList();

            return result;
        }

        public List<ReportSlottimeBooking> GetSlottimeBookingActual(string warehouseCode, string companyCode, DateTime fromDate)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vBookingDate = new OracleParameter("V_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);
            vBookingDate.Value = fromDate;


            var result = this.context.Set<ReportSlottimeBooking>()
                .FromSqlRaw("BEGIN RPT_SLOTTIME_BOOKING_ACTUAL (:V_WAREHOUSE_CODE,:V_COMPANY_CODE,:V_BOOKING_DATE,:V_CURSOR);  END;", new object[] { vWarehouseCode, vCompany, vBookingDate, vCursor })
                .ToList();

            return result;
        }

        public List<ReportSlottimeBooking> GetSlottimeBookingPending(string warehouseCode, string companyCode, DateTime fromDate)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vBookingDate = new OracleParameter("V_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);
            vBookingDate.Value = fromDate;


            var result = this.context.Set<ReportSlottimeBooking>()
                .FromSqlRaw("BEGIN RPT_SLOTTIME_BOOKING_PENDING (:V_WAREHOUSE_CODE,:V_COMPANY_CODE,:V_BOOKING_DATE,:V_CURSOR);  END;", new object[] { vWarehouseCode, vCompany, vBookingDate, vCursor })
                .ToList();

            return result;
        }

        public List<ReportDockDoorControl> GetDockDoorControl(string warehouseCode,string doorRange)
        {
            string from = "-";
            string to = "-";

            if(doorRange != "ALL")
            {
                var doors = doorRange.Split("-");
                if(doors.Length == 2 )
                {
                    from = doors[0];
                    to = doors[1];
                }
                else
                {
                    from = doorRange;
                    to = doorRange;
                }
            }

            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWarehouseCode = new OracleParameter("V_WAREHOUSE_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouseCode.Value = warehouseCode;
            var vDoorRange = new OracleParameter("V_DOOR_RANGE_FROM", OracleDbType.NVarchar2, ParameterDirection.Input);
            vDoorRange.Value = from;
            var vDoorRangeTo = new OracleParameter("V_DOOR_RANGE_TO", OracleDbType.NVarchar2, ParameterDirection.Input);
            vDoorRangeTo.Value = to;


            var result = this.context.Set<ReportDockDoorControl>()
                .FromSqlRaw("BEGIN RPT_DOCKDOOR_CONTROL (:V_WAREHOUSE_CODE,:V_DOOR_RANGE_FROM,:V_DOOR_RANGE_TO,:V_CURSOR);  END;", new object[] { vWarehouseCode, vDoorRange,vDoorRangeTo, vCursor })
                .ToList();

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

    }
}
