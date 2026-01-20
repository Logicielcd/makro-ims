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
    public class BookingKeyRepository : IBookingKeyRepository
    {
        private IMSContext context;
        private DbSet<BookingKey> dbSet;

        public BookingKeyRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingKeys;
        }

        public IEnumerable<BookingKey> GetBookingKeys()
        {
            return this.dbSet.ToList();
        }

        public BookingKey? GetBookingKeyById(int keyId)
        {
            return this.dbSet.FirstOrDefault(x=>x.InternalKeyId == keyId);
        }

        public IEnumerable<BookingKey>? GetBookingKeyByBookingDate(DateTime bookingDate)
        {
            return this.dbSet.Where(x=>x.BookingDate == bookingDate).ToList();
        }
      
        public bool Add(BookingKey bookingKey)
        {
            bool result = true;

            this.dbSet.Add(bookingKey);

            return result;
        }

        public bool Remove(BookingKey bookingKey)
        {
            bool result = true;

            this.dbSet.Remove(bookingKey);

            return result;
        }

        public bool Update(BookingKey bookingKey)
        {
            bool result = true;

            this.dbSet.Update(bookingKey);

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
