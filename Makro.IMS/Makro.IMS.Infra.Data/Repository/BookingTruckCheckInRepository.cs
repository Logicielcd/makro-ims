using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class BookingTruckCheckInRepository : IBookingTruckCheckInRepository
    {
        private IMSContext context;
        private DbSet<BookingTruckCheckIn> dbSet;

        public BookingTruckCheckInRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingTruckCheckIns;
        }

        public IEnumerable<BookingTruckCheckIn> GetBookingCheckInByHeaderId(int bookingHdrId)
        {
            return this.dbSet.Where(x=>x.InternalHeaderKey == bookingHdrId).ToList();
        }


        public BookingTruckCheckIn GetBookingCheckInById(int id)
        {
            return this.dbSet.FirstOrDefault(x => x.InternalTruckCheckInId == id);
        }

        public bool Add(BookingTruckCheckIn bookingKey)
        {
            bool result = true;

            try
            {
                this.context.Database.ExecuteSqlRaw("insert into booking_truck_check_in (internal_truck_check_in_id,internal_header_key,internal_truck_id,license_plate,check_in_time,user_stamp,driver_name,tel_no,line_id,status,license_plate_2) " +
                    "values (" + bookingKey.InternalTruckCheckInId.ToString() + "," + bookingKey.InternalHeaderKey.ToString() + "," + bookingKey.InternalTruckId.ToString() + ",'" + bookingKey.LicensePlate + "',sysdate,'" + 
                    bookingKey.UserStamp + "','" + bookingKey.DriverName + "','" + bookingKey.TelNo + "','" + bookingKey.LineId + "','" + bookingKey.Status + "','" + bookingKey.LicensePlate2 + "')");
            }
            catch
            {
                result = false;
            }
            
            return result;
        }

        public bool AddManualBooking(BookingTruckCheckIn bookingKey)
        {
            bool result = true;

            try
            {
                this.context.Database.ExecuteSqlRaw("insert into booking_truck_check_in (internal_truck_check_in_id,internal_header_key,internal_truck_id,license_plate,license_plate_2,check_in_time,user_stamp,driver_name,tel_no,line_id,status,date_time_stamp,arrived_time) " +
                    "values (" + bookingKey.InternalTruckCheckInId.ToString() + "," + bookingKey.InternalHeaderKey.ToString() + "," + bookingKey.InternalTruckId.ToString() + ",'" + bookingKey.LicensePlate + "','" + bookingKey.LicensePlate2 + "',sysdate,'" +
                    bookingKey.UserStamp + "','" + bookingKey.DriverName + "','" + bookingKey.TelNo + "','" + bookingKey.LineId + "','" + bookingKey.Status + "',sysdate,to_date('" + bookingKey.ArrivedTime.Value.ToString("yyyyMMdd HHmmss") + "','yyyyMMdd HH24:mi:ss'))");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool AddInboundBooking(BookingTruckCheckIn bookingKey)
        {
            bool result = true;

            try
            {
                this.context.Database.ExecuteSqlRaw("insert into booking_truck_check_in (internal_truck_check_in_id,internal_header_key,internal_truck_id,license_plate,license_plate_2,check_in_time,user_stamp,driver_name,tel_no,line_id,status,date_time_stamp) " +
                    "values (" + bookingKey.InternalTruckCheckInId.ToString() + "," + bookingKey.InternalHeaderKey.ToString() + "," + bookingKey.InternalTruckId.ToString() + ",'" + bookingKey.LicensePlate + "','" + bookingKey.LicensePlate2 + "',sysdate,'" +
                    bookingKey.UserStamp + "','" + bookingKey.DriverName + "','" + bookingKey.TelNo + "','" + bookingKey.LineId + "','" + bookingKey.Status + "',sysdate)");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool Remove(BookingTruckCheckIn bookingKey)
        {
            bool result = true;

            this.dbSet.Remove(bookingKey);

            return result;
        }

        public bool Remove(decimal id)
        {
            bool result = true;

            this.context.Database.ExecuteSqlRaw("delete from booking_truck_check_in where internal_truck_check_in_id = " + id.ToString());

            return result;
        }

        public bool Update(BookingTruckCheckIn bookingKey)
        {
            bool result = true;

            this.dbSet.Update(bookingKey);

            return result;
        }

        public List<BookingTruckCheckIn> GetLastQueueId(DateTime assignQueuDate)
        {
            var result = this.dbSet.Where(x => x.AssignQueueTime.Value.Date == assignQueuDate.Date).ToList();

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
