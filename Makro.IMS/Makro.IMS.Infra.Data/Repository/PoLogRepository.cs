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
    public class PoLogRepository : IPoLogRepository
    {
        private IMSContext context;
        private DbSet<PoLog> dbSet;

        public PoLogRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.PoLogs;
        }

        public IEnumerable<PoLog> GetPoLogByPoNo(string poNo)
        {
            return this.dbSet.Where(x => x.PoNbr == poNo).ToList();
        }

        public PoLog GetPoLogById(int id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public bool Add(PoLog poLog)
        {
            this.dbSet.Add(poLog);
            return true;
        }

        public bool Remove(PoLog poLog)
        {
            this.dbSet.Remove(poLog);
            return true;
        }

        public bool Update(PoLog poLog)
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
