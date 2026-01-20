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
    public class OperationRepository : IOperationRepository
    {
        private IMSContext context;
        private DbSet<Operation> dbSet;

        public OperationRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Operations;
        }

        
        public IEnumerable<Operation> GetOperations()
        {
            return this.dbSet.ToList();
        }

        public IQueryable<Operation> GetOperationPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public IEnumerable<Operation> GetOperationsByWarehouse(string warehouseCode)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehouseCode).ToList();
        }

        public Operation? GetOperationByNameAndWhse(string operationName, string warehouseCode)
        {
            return this.dbSet.FirstOrDefault(x => x.OperationName == operationName && x.WarehouseCode == warehouseCode);
        }

        public void Add(Operation data)
        {
            this.dbSet.Add(data);
        }

        public void Update(Operation data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(Operation data)
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
