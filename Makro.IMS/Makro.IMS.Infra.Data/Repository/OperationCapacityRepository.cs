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
    public class OperationCapacityRepository : IOperationCapacityRepository
    {
        private IMSContext context;
        private DbSet<OperationCapacity> dbSet;

        public OperationCapacityRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.OperationCapacities;
        }

       
        public List<OperationCapacity> GetOperationByNameAndWhse(string operationName, string warehouseCode)
        {
            return this.dbSet.Where(x => x.OperationType == operationName && x.WarehouseCode == warehouseCode).ToList();
        }


        public OperationCapacity? GetOperationByNameAndWhseAndTime(string operationName, string warehouseCode, DateTime time)
        {
            return this.dbSet.FirstOrDefault(x => x.OperationType == operationName && x.WarehouseCode == warehouseCode && x.Time.Value.Hour == time.Hour && x.Time.Value.Minute == time.Minute);
        }


        public void Add(OperationCapacity data)
        {
            this.dbSet.Add(data);
        }

        public void Update(OperationCapacity data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(OperationCapacity data)
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
