using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class PoListRepository : IDisposable
    {
        private IMSContext context;        

        public PoListRepository(IMSContext context)
        {
            this.context = context;            
        }

        public IEnumerable<PoList> GetPoList(string supCode,string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vSupcode = new OracleParameter("V_SUP_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);            
            vSupcode.Value = supCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN MT_PO_LIST_GET (:V_SUP_CODE,:V_COMPANY_CODE,:V_CURSOR);  END;", new object[] {vSupcode,vCompany,vCursor})
                .ToList();

            return poList;
        }
       
        public IEnumerable<PoList> GetPoNotBooking(string supCode, string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vSupcode = new OracleParameter("V_SUP_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vSupcode.Value = supCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN MT_PO_LIST_GET (:V_SUP_CODE,:V_COMPANY_CODE,:V_CURSOR);  END;", new object[] { vSupcode, vCompany, vCursor })
                .ToList();

            //return poList.Where(x=>x.Booking_Id is null).ToList();
            return poList.ToList();
        }

        public PoList GetPoByPoNo(string poNo, string supCode, string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vSupcode = new OracleParameter("V_SUP_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vSupcode.Value = supCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vPoNbr = new OracleParameter("v_PO_NBR", OracleDbType.NVarchar2, ParameterDirection.Input);
            vPoNbr.Value = poNo;

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN MT_PO_LIST_GET_ONLINE (:V_SUP_CODE,:V_COMPANY_CODE,:V_PO_NBR,:V_CURSOR);  END;", new object[] { vSupcode, vCompany,vPoNbr, vCursor })
                .ToList();

            if(poList.Count == 0)
            {
                return null;
            }

            return poList.FirstOrDefault();

        }

        public List<PoList> GetPoByPo(string poNo)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);                
            var vPoNbr = new OracleParameter("v_PO_NBR", OracleDbType.NVarchar2, ParameterDirection.Input);
            vPoNbr.Value = poNo;

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN MT_PO_LIST_GET_BY_PO (:V_PO_NBR,:V_CURSOR);  END;", new object[] { vPoNbr, vCursor })
                .ToList();

            if(poList.Count == 0)
            {
                return null;
            }

            return poList;

        }

        public IEnumerable<PoList> GetPoByPos(string poNo, string supCode, string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vSupcode = new OracleParameter("V_SUP_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vSupcode.Value = supCode;
            var vCompany = new OracleParameter("V_COMPANY_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vCompany.Value = companyCode;
            var vPoNbr = new OracleParameter("v_PO_NBR", OracleDbType.NVarchar2, ParameterDirection.Input);
            vPoNbr.Value = poNo;

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN MT_PO_LIST_GET_BY_LIST (:V_SUP_CODE,:V_COMPANY_CODE,:V_PO_NBR,:V_CURSOR);  END;", new object[] { vSupcode, vCompany, vPoNbr, vCursor })
                .ToList();

            if (poList.Count == 0)
            {
                return null;
            }

            return poList;
        }

        public IEnumerable<BookingCapacity> GetCapacity(DateTime bookingDate, string warehouseCode)
        {
            var vCursor = new OracleParameter("v_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vBookingDate = new OracleParameter("v_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);
            vBookingDate.Value = bookingDate;
            var vWarehouse = new OracleParameter("v_WHSE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouse.Value = warehouseCode;
            var vInternalKeyId = new OracleParameter("v_INTERNAL_KEY_ID", OracleDbType.Int32, ParameterDirection.Input);
            vInternalKeyId.Value = 0;


            var poList = this.context.Set<BookingCapacity>()
                .FromSqlRaw("BEGIN GET_CON_NON_FULL_BY_DATE (:v_BOOKING_DATE,:v_WHSE,:v_INTERNAL_KEY_ID,:V_CURSOR);  END;", new object[] { vBookingDate, vWarehouse,vInternalKeyId, vCursor })
                .ToList();

            return poList;
        }

        public IEnumerable<BookingCapacity> GetCapacityOperation(DateTime bookingDate, string warehouseCode,string operationType)
        {
            var vCursor = new OracleParameter("v_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vBookingDate = new OracleParameter("v_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);
            vBookingDate.Value = bookingDate;
            var vWarehouse = new OracleParameter("v_WHSE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vWarehouse.Value = warehouseCode;
            var vOps = new OracleParameter("v_OPS", OracleDbType.NVarchar2, ParameterDirection.Input);
            vOps.Value = operationType;
            var vInternalKeyId = new OracleParameter("v_INTERNAL_KEY_ID", OracleDbType.Int32, ParameterDirection.Input);
            vInternalKeyId.Value = 0;


            var poList = this.context.Set<BookingCapacity>()
                .FromSqlRaw("BEGIN GET_CAP_OPERATION_BY_DATE (:v_BOOKING_DATE,:v_WHSE,:v_OPS,:v_INTERNAL_KEY_ID,:V_CURSOR);  END;", new object[] { vBookingDate, vWarehouse,vOps, vInternalKeyId, vCursor })
                .ToList();

            return poList;
        }

        public int GetBookingKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            

            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_BOOKING_KEY (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();            

            return keyId[0].Nextval;
        }

        public int GetBookingHeaderKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_BOOKING_HEADER (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();

            return keyId[0].Nextval;
        }
        
        public int GetBookingDetailKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_BOOKING_DETAIL (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();

            return keyId[0].Nextval;
        }

        public int GetBookingTruckCheckInKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_BOOKING_TRUCK_CHECK_IN (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();

            return keyId[0].Nextval;
        }

        public int GetBookingTruckCheckInDtlKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_BOOKING_TRUCK_CHECK_IN_DTL (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();

            return keyId[0].Nextval;
        }

        public List<BookingCheckInDto> GetBookingCheckIn(int internalSupGroup)
        {
            string sqlCmd = "select bh.internal_header_key as internalHeaderKey,bd.internal_detail_key as internalDetailKey,bd.po_nbr as poNbr," +
                "bh.warehouse_code as warehouseCode,bh.booking_id as bookingId,tm.truck_name as truckType,bc.driver_name as driverName," +
                "bc.license_plate as licensePlate,bc.tel_no as telNo,bh.booking_start as bookingDate,bc.line_id as lineId, " +
                "bh.merch_type as merchType " +
                "from booking_header bh inner join booking_detail bd on bh.internal_header_key = bd.internal_header_key " +
                "inner join booking_truck_check_in_detail bcd on bd.internal_detail_key = bcd.internal_detail_key " +
                "inner join booking_truck_check_in bc on bh.internal_header_key = bc.internal_header_key and bcd.internal_truck_check_in_id = bc.internal_truck_check_in_id " +
                "inner join truck_master tm on bc.internal_truck_id = tm.internal_truck_id " +
                "where bd.status = 'INTRANSIT' and bh.internal_sup_group_id = " + internalSupGroup.ToString() + ";";

            var result = this.context.Set<BookingCheckInDto>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

        public List<BookingCheckInDto> GetBookingDocumentCheckIn(int internalSupGroup)
        {
            string sqlCmd = "select bh.internal_header_key as internalHeaderKey,bd.internal_detail_key as internalDetailKey,bd.po_nbr as poNbr," +
                "bh.warehouse_code as warehouseCode,bh.booking_id as bookingId,tm.truck_name as truckType,bc.driver_name as driverName," +
                "bc.license_plate as licensePlate,bc.tel_no as telNo,bh.booking_start as bookingDate,bc.line_id as lineId, " +
                "bh.merch_type as merchType " +
                "from booking_header bh inner join booking_detail bd on bh.internal_header_key = bd.internal_header_key " +
                "inner join booking_truck_check_in_detail bcd on bd.internal_detail_key = bcd.internal_detail_key " +
                "inner join booking_truck_check_in bc on bh.internal_header_key = bc.internal_header_key and bcd.internal_truck_check_in_id = bc.internal_truck_check_in_id " +
                "inner join truck_master tm on bc.internal_truck_id = tm.internal_truck_id " +
                "where bd.status = 'CHECKIN' and bh.internal_sup_group_id = " + internalSupGroup.ToString() + ";";

            var result = this.context.Set<BookingCheckInDto>()
                                            .FromSqlRaw(sqlCmd)
                                            .ToList();

            return result;
        }

        // Update MT_PO_LIST
        public void UpdatePoPostponed(string poNbr,string delayReason)
        {
            string sqlCmd = "update mt_po_list set is_delay = 1,delay_reason = '" + delayReason + "' where po_nbr = '" + poNbr + "'";
            this.context.Database.ExecuteSqlRawAsync(sqlCmd);            
        }

        public void UpdatePoPostponedInBookingDetail(string poNbr, string delayReason)
        {
            string sqlCmd = "update booking_detail set delay_reason = '" + delayReason + "' where po_nbr = '" + poNbr + "'";
            this.context.Database.ExecuteSqlRawAsync(sqlCmd);
        }

        public int CheckDuplicateTruck(DateTime bookingDate,string bookingId,string truckLicensePlate)
        {
            var vCursor = new OracleParameter("v_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vBookingDate = new OracleParameter("v_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);
            vBookingDate.Value = bookingDate;
            var vLicensePlate = new OracleParameter("v_TRUCK_LICENSE_PLATE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vLicensePlate.Value = truckLicensePlate;
            var vBookingId = new OracleParameter("v_BOOKING_ID", OracleDbType.NVarchar2, ParameterDirection.Input);
            vBookingId.Value = bookingId;

            var poList = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN CHECK_DUPLICATE_TRUCK (:v_BOOKING_DATE,:v_TRUCK_LICENSE_PLATE,:v_BOOKING_ID,:V_CURSOR);  END;", new object[] { vBookingDate, vLicensePlate,vBookingId, vCursor })
                .ToList();

            return poList[0].Nextval;
        }

        public IEnumerable<PoMonitor> GetPoMonitor(string userId)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vUserId = new OracleParameter("V_USER_ID", OracleDbType.NVarchar2, ParameterDirection.Input);
            vUserId.Value = userId;
            
            var poMonitors = this.context.Set<PoMonitor>()
                .FromSqlRaw("BEGIN RPT_PO_MONITOR (:V_USER_ID,:V_CURSOR);  END;", new object[] { vUserId, vCursor })
                .ToList();

            return poMonitors;
        }


        #region +++ CDC +++

        public PoList GetPoByPoNoCDC(string poNo, string supCode, string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vSupcode = new OracleParameter("V_SUP_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vSupcode.Value = supCode;            
            var vPoNbr = new OracleParameter("v_PO_NBR", OracleDbType.NVarchar2, ParameterDirection.Input);
            vPoNbr.Value = poNo;

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN CDC_SP001_INB_BOOKING (:V_PO_NBR,:V_SUP_CODE,:V_CURSOR);  END;", new object[] { vPoNbr, vSupcode, vCursor })
                .ToList();

            if (poList.Count == 0)
            {
                return null;
            }

            return poList.FirstOrDefault();

        }

        public IEnumerable<PoList> GetPoNotBookingCDC(string supCode, string companyCode)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vSupcode = new OracleParameter("V_SUP_CODE", OracleDbType.NVarchar2, ParameterDirection.Input);
            vSupcode.Value = supCode;            

            var poList = this.context.Set<PoList>()
                .FromSqlRaw("BEGIN CDC_SP001_INB_BOOKING_ALL (:V_SUP_CODE,:V_CURSOR);  END;", new object[] { vSupcode, vCursor })
                .ToList();

            return poList.Where(x => x.Booking_Id is null).ToList();
        }

        #endregion

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
