using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class BookingTruckRepository : IBookingTruckRepository
    {
        private IMSContext context;
        private DbSet<BookingTruck> dbSet;

        public BookingTruckRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingTrucks;
        }

        public IEnumerable<BookingTruck> GetBookingTrucksByHeaderId(int bookingHdrId)
        {
            return this.dbSet.Where(x => x.InternalHeaderKey == bookingHdrId).ToList();
        }

        public BookingTruck? GetBookingTruckById(int bookingHdrId, int truckId)
        {
            return this.dbSet.FirstOrDefault(x => x.InternalHeaderKey == bookingHdrId && x.InternalTruckId == truckId);
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

        public bool Add(BookingTruck bookingTruck)
        {
            bool result = true;

            try
            {
                this.context.Database.ExecuteSqlRaw("insert into booking_truck (internal_header_key,internal_truck_id,total_truck,remark) values (" + bookingTruck.InternalHeaderKey.ToString() 
                    + "," + bookingTruck.InternalTruckId.ToString() + "," + bookingTruck.TotalTruck.Value.ToString() + ",'')");
            }
            catch 
            {
                result = false;
            }
            return result;
        }

        public bool Remove(BookingTruck bookingTruck)
        {
            bool result = true;

            try
            {
                this.context.Database.ExecuteSqlRaw("delete from booking_truck where internal_header_key = " + bookingTruck.InternalHeaderKey.ToString());
            }
            catch
            {
                result = false;
            }
            //this.dbSet.Remove(bookingTruck);

            return result;
        }

        public bool Update(BookingTruck bookingTruck)
        {
            bool result = true;

            this.dbSet.Update(bookingTruck);

            return result;
        }
    }
}
