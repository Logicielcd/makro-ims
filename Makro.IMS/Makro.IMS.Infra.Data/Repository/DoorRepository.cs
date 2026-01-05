using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class DoorRepository : IDoorRepository
    {
        private IMSContext context;
        private DbSet<Door> dbSet;

        public DoorRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Doors;
        }
        public IEnumerable<Door> GetDoors()
        {
            return this.dbSet.ToList();
        }

        public Door? GetDoorById(int doorId)
        {
            return this.dbSet.FirstOrDefault(x => x.InternalDoorId == doorId);
        }

        public Door? GetDoorByName(string doorName)
        {
            return this.dbSet.FirstOrDefault(x => x.DoorName == doorName);
        }

        public void Add(Door door)
        {

            this.dbSet.Add(door);
        }

        public void Update(Door door)
        {
            this.dbSet.Update(door);
        }

        public void Delete(Door door)
        {
            this.dbSet.Remove(door);
        }

        public int GetDoorKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_DOOR (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();

            return keyId[0].Nextval;
        }

        public IEnumerable<Door> GetDoorsByWarehouse(string warehoueCode)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehoueCode).ToList();
        }

        public IEnumerable<Door> GetDoorsByDoorType(string warehoueCode, string doorType)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehoueCode && x.LoadingType == doorType).ToList();
        }

        public IEnumerable<Door> GetDoorsAvailableByOperationType(string warehoueCode, string operationType, string truckType)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehoueCode && x.DoorArea.Contains(operationType)
            && x.BookingHeaderKey == null
            && x.TruckType.Contains(truckType)
            ).ToList();
        }


        public IEnumerable<Door> GetDoorAvailable(DateTime bookingDate, DateTime bookingStart, DateTime bookingEnd, string warehouseCode, string doorType)
        {
            bookingStart = bookingStart.AddMinutes(1);
            bookingEnd = bookingEnd.AddMinutes(-1);

            string sqlCmd = "";
            sqlCmd = "select * from door where internal_door_id not in (select internal_door_id from booking_header where internal_key_id " +
                "in (select internal_key_id from booking_key where booking_date = to_date('" + bookingDate.ToString("dd-MM-yyyy") + "','DD-MM-YYYY') ) " +
                "and (to_date('" + bookingStart.ToString("dd-MM-yyyy HH:mm:ss") + "','DD-MM-YYYY HH24:MI:SS') between booking_start and booking_end " +
                "or to_date('" + bookingEnd.ToString("dd-MM-yyyy HH:mm:ss") + "','DD-MM-YYYY HH24:MI:SS') between booking_start and booking_end) " +
                "and warehouse_code = '" + warehouseCode + "') and warehouse_code = '" + warehouseCode + "' and loading_type = '" + doorType + "';";

            var result = this.dbSet.FromSqlRaw(sqlCmd);

            return result;
        }

        public IEnumerable<Door> GetDoorAvailable(DateTime bookingDate, DateTime bookingStart, DateTime bookingEnd, string warehouseCode, string doorType, string bookingId)
        {
            bookingStart = bookingStart.AddMinutes(1);
            bookingEnd = bookingEnd.AddMinutes(-1);

            string sqlCmd = "";
            sqlCmd = "select * from door where internal_door_id not in (select internal_door_id from booking_header where internal_key_id " +
                "in (select internal_key_id from booking_key where booking_date = to_date('" + bookingDate.ToString("dd-MM-yyyy") + "','DD-MM-YYYY') ) " +
                "and (to_date('" + bookingStart.ToString("dd-MM-yyyy HH:mm:ss") + "','DD-MM-YYYY HH24:MI:SS') between booking_start and booking_end " +
                "or to_date('" + bookingEnd.ToString("dd-MM-yyyy HH:mm:ss") + "','DD-MM-YYYY HH24:MI:SS') between booking_start and booking_end) " +
                "and warehouse_code = '" + warehouseCode + "' and booking_id != '" + bookingId + "') and warehouse_code = '" + warehouseCode + "' and loading_type = '" + doorType + "';";

            var result = this.dbSet.FromSqlRaw(sqlCmd);

            return result;
        }

        public IQueryable<Door> GetDoorPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public IEnumerable<DoorQueueDto> GetDoorQueues(string warehouseCode)
        {
            string sqlCmd = "";

            sqlCmd = "select distinct d.internal_door_id as InternalDoorId,door_name as DoorName,door_area as DoorArea,"
               + "truck_type as TruckType,bh.internal_header_key as BookingHeaderKey,bh.booking_id as BookingId,"
               + "bh.sup_code as SupCode,bh.sup_name as SupName, bt.arrived_time as ArrivedTime,"
               + "bt.submitdoc_time as SubmitDocTime,ondock_time as OnDockTime,start_unload_time as StartUnloadTime,"
               + "finish_unload_time as FinishUnloadTime,departure_time as DepartureTime,d.sequence,bt.license_plate as LicensePlate,nvl(bt.license_plate_2,' ') as LicensePlate2, "
               + "bt.internal_truck_check_in_id as InternalTruckCheckInId,calltruck_time as CalltruckTime,Checkout_Time as CheckoutTime,"
               + "Doccheck_Time as DoccheckTime,tm.truck_code as BookingTruckType,"
               + "nvl(round((sysdate - bt.date_time_stamp) * 24 * 60 * 60, 0),0) as WaitingTime,bt.date_time_stamp "
               + "from door d "
               + "left join booking_truck_check_in bt on d.booking_header_key = bt.internal_truck_check_in_id "
               + "left join booking_header bh on bt.internal_header_key = bh.internal_header_key "
               + "left join truck_master tm on bt.internal_truck_id = tm.internal_truck_id "
               + "where d.warehouse_code ='" + warehouseCode + "' "
               //+ "order by bt.date_time_stamp; ";
               + "order by door_area, d.sequence; ";

            var result = this.context.Set<DoorQueueDto>()
                        .FromSqlRaw(sqlCmd)
                        .ToList();

            return result;
        }

        public IEnumerable<DoorQueueDto> GetDoorQueuesByOperationtype(string warehouseCode, string operationType)
        {
            string sqlCmd = "";

            sqlCmd = "select distinct d.internal_door_id as InternalDoorId,door_name as DoorName,door_area as DoorArea,"
               + "truck_type as TruckType,bh.internal_header_key as BookingHeaderKey,bh.booking_id as BookingId,"
               + "bh.sup_code as SupCode,bh.sup_name as SupName, bt.arrived_time as ArrivedTime,"
               + "bt.submitdoc_time as SubmitDocTime,ondock_time as OnDockTime,start_unload_time as StartUnloadTime,"
               + "finish_unload_time as FinishUnloadTime,departure_time as DepartureTime,d.sequence,bt.license_plate as LicensePlate,nvl(bt.license_plate_2,' ') as LicensePlate2, "
               + "bt.internal_truck_check_in_id as InternalTruckCheckInId,calltruck_time as CalltruckTime,Checkout_Time as CheckoutTime,"
               + "Doccheck_Time as DoccheckTime,tm.truck_code as BookingTruckType,"
               + "nvl(round((sysdate - bt.date_time_stamp) * 24 * 60 * 60, 0),0) as WaitingTime,bt.date_time_stamp "
               + "from door d "
               + "left join booking_truck_check_in bt on d.booking_header_key = bt.internal_truck_check_in_id "
               + "left join booking_header bh on bt.internal_header_key = bh.internal_header_key "
               + "left join truck_master tm on bt.internal_truck_id = tm.internal_truck_id "
               + "where d.warehouse_code ='" + warehouseCode + "' and d.door_area like '%" + operationType + "%' "
                //+ "order by bt.date_time_stamp; ";
                + "order by door_area, d.sequence; ";

            var result = this.context.Set<DoorQueueDto>()
                        .FromSqlRaw(sqlCmd)
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
