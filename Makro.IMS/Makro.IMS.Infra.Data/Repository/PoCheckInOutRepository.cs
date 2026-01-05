using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class PoCheckInOutRepository : IPoCheckInOutRepository
    {
        private IMSContext context;
        private DbSet<PoCheckInOut> dbSet;

        public PoCheckInOutRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.PoCheckInOuts;
        }
        public IEnumerable<PoCheckInOut> GetPoCheckInOuts()
        {
            return this.dbSet.ToList();
        }

        public IEnumerable<PoCheckInOut> GetPoCheckInOutsByInternalTruckCheckInId(int id)
        {
            return this.dbSet.Where(x=>x.InternalTruckCheckinId == id && x.Status == "CHECKIN").ToList();
        }

        public IEnumerable<PoCheckInOut> GetPoCheckOutsByInternalTruckCheckInId(int id)
        {
            return this.dbSet.Where(x => x.InternalTruckCheckinId == id && x.Status == "CHECKOUT").ToList();
        }

        public PoCheckInOut? GetPoCheckInOut(int id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public PoCheckInOut? GetPoCheckIn(string poNbr)
        {
            return this.dbSet.FirstOrDefault(x => x.PoNbr == poNbr && x.Status == "CHECKIN");
        }

        public PoCheckInOut? GetPoCheckOut(string poNbr)
        {
            return this.dbSet.FirstOrDefault(x => x.PoNbr == poNbr && x.Status == "CHECKOUT");
        }


        public void Add(PoCheckInOut poCheckInOut)
        {
            this.dbSet.Add(poCheckInOut);
        }

        public void Update(PoCheckInOut poCheckInOut)
        {
            this.dbSet.Update(poCheckInOut);
        }

        public void Delete(PoCheckInOut poCheckInOut)
        {
            this.dbSet.Remove(poCheckInOut);
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
