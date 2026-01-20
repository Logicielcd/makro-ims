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
    public class WarehouseRepository : IWarehouseRepository
    {
        private IMSContext context;
        private DbSet<Warehouse> dbSet;

        public WarehouseRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Warehouses;
        }
        public IEnumerable<Warehouse> GetWarehouses()
        {
            return this.dbSet.ToList();
        }
        public IQueryable<Warehouse> GetWarehousesPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public Warehouse? GetWarehouseByWhseCode(string whseCode)
        {
            return this.dbSet.FirstOrDefault(x => x.WarehouseCode == whseCode);
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

        public void Update(Warehouse warehouse)
        {
            this.context.Database.ExecuteSqlRaw("update warehouse set online_booking_id_running = " + warehouse.OnlineBookingIdRunning.ToString() + " where warehouse_code = '" + warehouse.WarehouseCode + "'");            
        }

        public void UpdateWarehouse(Warehouse warehouse)
        {
            this.dbSet.Update(warehouse);
        }

        public void Add(Warehouse warehouse)
        {
            this.dbSet.Add(warehouse);
        }

        public void Delete(Warehouse warehouse)
        {
            this.dbSet.Remove(warehouse);
        }
    }
}
