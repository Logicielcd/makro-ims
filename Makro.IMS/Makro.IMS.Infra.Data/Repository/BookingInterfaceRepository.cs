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
    public class BookingInterfaceRepository : IBookingInterfaceRepository
    {
        private IMSContext context;
        private DbSet<BookingInterface> dbSet;

        public BookingInterfaceRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingInterfaces;
        }

        public IEnumerable<BookingInterface> GetBookingInterfaceByBookingId(string bookingId)
        {
            return this.dbSet.Where(x => x.BookingId == bookingId).ToList();
        }

        public BookingInterface GetBookingInterfaceById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Add(BookingInterface poLog)
        {
            this.dbSet.Add(poLog);
            return true;
        }

        public bool Remove(BookingInterface poLog)
        {
            this.dbSet.Remove(poLog);
            return true;
        }

        public bool Update(BookingInterface poLog)
        {
            this.dbSet.Update(poLog);
            return true;
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
