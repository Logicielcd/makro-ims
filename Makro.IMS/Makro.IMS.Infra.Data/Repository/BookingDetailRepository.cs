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
    public class BookingDetailRepository : IBookingDetailRepository
    {
        private IMSContext context;
        private DbSet<BookingDetail> dbSet;

        public BookingDetailRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingDetails;
        }

        public IEnumerable<BookingDetail> GetBookingDetailsByHeaderId(int bookingHdrId)
        {
            return this.dbSet.Where(x => x.InternalHeaderKey == bookingHdrId).OrderBy(x=>x.InternalDetailKey).ToList();
        }

        public BookingDetail? GetBookingDetailById(int detailId)
        {
            return this.dbSet.Where(x=>x.InternalDetailKey == detailId).FirstOrDefault();
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

        public bool Add(BookingDetail bookingDetail)
        {
            bool result = true;

            this.dbSet.Add(bookingDetail);

            return result;
        }

        public bool Remove(BookingDetail bookingDetail)
        {
            bool result = true;

            this.dbSet.Remove(bookingDetail);

            return result;
        }

        public bool Update(BookingDetail bookingDetail)
        {
            bool result = true;

            this.dbSet.Update(bookingDetail);

            return result;
        }

        public BookingDetail GetBookingDetailByPoNo(string poNo)
        {
            return this.dbSet.FirstOrDefault(x => x.PoNbr == poNo);
        }

        public List<BookingDetail> GetBookingDetailsByPoNo(string poNo)
        {
            return this.dbSet.Where(x => x.PoNbr == poNo).ToList();
        }


        public BookingDetail GetBookingDetailByPoNoAndId(string poNo, int bookingHdrId)
        {
            return this.dbSet.FirstOrDefault(x => x.PoNbr == poNo && x.InternalHeaderKey == bookingHdrId);
        }
    }
}
