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
    public class OperationFixSlotRepository : IOperationFixSlotRepository
    {
        private IMSContext context;
        private DbSet<OperationFixSlot> dbSet;

        public OperationFixSlotRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.OperationFixSlots;
        }

        public IEnumerable<OperationFixSlot> GetOperationFixSlotByWhseAndOpType(string warehouseCode, string operationName)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehouseCode && x.OperationType == operationName).ToList();
        }

        public IEnumerable<OperationFixSlot> GetOperationFixSlotByWhseAndOpTypeAndSupGroup(string warehouseCode, string operationName, decimal supGroupId)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehouseCode 
            && x.OperationType == operationName
            && x.SupGroupId == supGroupId).ToList();
        }

        public OperationFixSlot? GetOperationById(decimal id)
        {
            return this.dbSet.FirstOrDefault(x=>x.Id == id);
        }

        public void Add(OperationFixSlot data)
        {
            dbSet.Add(data);
        }

        public void Update(OperationFixSlot data)
        {
            dbSet.Update(data);
        }

        public void Delete(OperationFixSlot data)
        {
            dbSet.Remove(data);
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
