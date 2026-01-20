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
    public class TruckMasterRepository : ITruckMasterRepository
    {
        private IMSContext context;
        private DbSet<TruckMaster> dbSet;

        public TruckMasterRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.TruckMasters;
        }

        public IEnumerable<TruckMaster> GetTrucks()
        {
            return this.dbSet.ToList();
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
