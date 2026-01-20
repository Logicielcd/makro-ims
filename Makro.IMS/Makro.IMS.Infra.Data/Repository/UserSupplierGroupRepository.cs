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
    public class UserSupplierGroupRepository : IUserSupplierGroupRepository
    {
        private IMSContext context;
        private DbSet<UserSupplierGroup> dbSet;

        public UserSupplierGroupRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.UserSupplierGroups;
        }

        public IEnumerable<UserSupplierGroup> GetUserSupplierByUserId(string userId)
        {
            return this.dbSet.Where(x => x.UserId == userId);
        }

        public void AddUserSupplierGroup(UserSupplierGroup userSupplierGroup)
        {
            this.dbSet.Add(userSupplierGroup);
        }

        public void DeleteUserSupplierGroup(UserSupplierGroup userSupplierGroup)
        {
            this.dbSet.Remove(userSupplierGroup);
        }

        public void AddRangeUserSupplierGroup(List<UserSupplierGroup> userSupplierGroups)
        {
            this.dbSet.AddRange(userSupplierGroups);
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
