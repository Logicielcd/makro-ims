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
    public class OperationTimeRepository : IOperationTimeRepository
    {
        private IMSContext context;
        private DbSet<OperationTime> dbSet;

        public OperationTimeRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.OperationTimes;
        }

        public IEnumerable<OperationTime> GetOperationTimes()
        {
            return this.dbSet.ToList();
        }

        public IQueryable<OperationTime> GetOperationTimesPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public IEnumerable<OperationTime> GetOperationTimesByWarehouse(string warehouseCode)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehouseCode).ToList();
        }

        public IEnumerable<OperationTime> GetOperationTimesByWarehouseByOperationType(string warehouseCode, string operationName)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehouseCode && x.OperationType == operationName).ToList();
        }

        public OperationTime? GetOperationById(decimal id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public void Add(OperationTime data)
        {
            this.dbSet.Add(data);
        }

        public void Update(OperationTime data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(OperationTime data)
        {
            this.dbSet.Remove(data);
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
