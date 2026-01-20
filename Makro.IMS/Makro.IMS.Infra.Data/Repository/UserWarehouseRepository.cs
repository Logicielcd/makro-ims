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
    public class UserWarehouseRepository : IUserWarehouseRepository
    {
        private IMSContext context;
        private DbSet<UserWarehouse> dbSet;

        public UserWarehouseRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.UserWarehouses;
        }

        public IEnumerable<UserWarehouse> GetUserWarehouseByUserId(string userId)
        {
            return this.dbSet.Where(x => x.UserId == userId);
        }

        public void AddUserWarehouse(UserWarehouse userWarehouse)
        {
            this.dbSet.Add(userWarehouse);
        }

        public void AddRangeUserWarehouse(List<UserWarehouse> userWarehouses)
        {
            this.dbSet.AddRange(userWarehouses);
        }

        public void DeleteUserWarehouse(UserWarehouse userWarehouse)
        {
            this.dbSet.Remove(userWarehouse);
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
