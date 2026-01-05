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
    public class BookingTruckLogRepository : IBookingTruckLogRepository
    {
        private IMSContext context;
        private DbSet<BookingTruckLog> dbSet;

        public BookingTruckLogRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingTruckLogs;
        }

        public IEnumerable<BookingTruckLog> GetBookingTruckLogByInternalTruckCheckinId(int internalTruckCheckinId)
        {
            return this.dbSet.Where(x=>x.InternalTruckCheckInId == internalTruckCheckinId).ToList();
        }


        public BookingTruckLog GetBookingTruckLogById(int id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }


        public bool Add(BookingTruckLog bookingTruckLog)
        {
            bool result = true;

            this.dbSet.Add(bookingTruckLog);
            
            return result;
        }

        public bool Remove(BookingTruckLog bookingTruckLog)
        {
            bool result = true;

            this.dbSet.Remove(bookingTruckLog);

            return result;
        }


        public bool Update(BookingTruckLog bookingTruckLog)
        {
            bool result = true;

            this.dbSet.Update(bookingTruckLog);

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
