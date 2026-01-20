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
    public class WarehouseOperationCapacityRepository : IWarehouseOperationCapacityRepository
    {
        private IMSContext context;
        private DbSet<WarehouseOperationCapacity> dbSet;

        public WarehouseOperationCapacityRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.WarehouseOperationCapacities;
        }

        public IEnumerable<WarehouseOperationCapacity> GetWarehousesCapacity()
        {
            return this.dbSet.ToList();
        }

        public WarehouseOperationCapacity? GetWarehouseCapacirtyById(int id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }
        public IQueryable<WarehouseOperationCapacity> GetWarehousesCapacityPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public IEnumerable<WarehouseOperationCapacity> GetByWarehouse(string warehouse)
        {
            return this.dbSet.Where(x => x.WarehouseCode == warehouse && x.BookingDate >= DateTime.Now.AddDays(-6) && x.BookingDate <= DateTime.Now.AddDays(30)).ToList();
        }
        public WarehouseOperationCapacity GetByWarehouseBookingDate(string warehouse,DateTime bookingDate)
        {
            return this.dbSet.FirstOrDefault(x => x.WarehouseCode == warehouse && x.BookingDate.Date == bookingDate.Date);
        }

        public void Add(WarehouseOperationCapacity warehouseCapacity)
        {
            this.dbSet.Add(warehouseCapacity);
        }

        public void Update(WarehouseOperationCapacity warehouseCapacity)
        {
            this.dbSet.Update(warehouseCapacity);
        }

        public void Remove(WarehouseOperationCapacity warehouseCapacity)
        {
            this.dbSet.Remove(warehouseCapacity);
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
