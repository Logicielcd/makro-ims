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
    public class BookingTruckCheckInDetailRepository : IBookingTruckCheckInDetailRepository
    {
        private IMSContext context;
        private DbSet<BookingTruckCheckInDetail> dbSet;

        public BookingTruckCheckInDetailRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.BookingTruckCheckInDetails;
        }

        public IEnumerable<BookingTruckCheckInDetail> GetByCheckInId(int checkInId)
        {
            return this.dbSet.Where(x=>x.InternalTruckCheckInId == checkInId).ToList();
        }

        public IEnumerable<BookingTruckCheckInDetail> GetByPo(int checkInId, string poNbr)
        {
            return this.dbSet.Where(x =>x.InternalTruckCheckInId == checkInId && x.PoNbr == poNbr).ToList();
        }

        
        public bool Add(BookingTruckCheckInDetail bookingCheckInDetail)
        {
            bool result = true;

            try
            {
                this.context.Database.ExecuteSqlRaw("insert into booking_truck_check_in_detail (internal_truck_detail_id, internal_truck_check_in_id,internal_detail_key,check_in_time,po_nbr) " +
                    "values (" + bookingCheckInDetail.InternalTruckDetailId.ToString() + "," + bookingCheckInDetail.InternalTruckCheckInId.Value.ToString() + "," + bookingCheckInDetail.InternalDetailKey.Value.ToString() + ",sysdate,'" + bookingCheckInDetail.PoNbr + "')");
            }
            catch
            {
                result = false;
            }
            
            return result;
        }

        public bool Remove(BookingTruckCheckInDetail bookingCheckInDetail)
        {
            bool result = true;

            this.context.Database.ExecuteSqlRaw("delete from booking_truck_check_in_detail where internal_truck_check_in_id = " + bookingCheckInDetail.InternalTruckCheckInId.Value.ToString());

            return result;
        }

        public bool Remove(decimal id)
        {
            bool result = true;

            this.context.Database.ExecuteSqlRaw("delete from booking_truck_check_in_detail where internal_truck_check_in_id = " + id.ToString());

            return result;
        }

        public bool Update(BookingTruckCheckInDetail bookingCheckInDetail)
        {
            bool result = true;

            this.dbSet.Update(bookingCheckInDetail);

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
