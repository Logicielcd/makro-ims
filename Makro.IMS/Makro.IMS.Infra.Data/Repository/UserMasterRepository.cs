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
    public class UserMasterRepository : IUserMasterRepository
    {
        private IMSContext context;
        private DbSet<UserMaster> dbSet;

        public UserMasterRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.UserMasters;
        }

        public IEnumerable<UserMaster> GetUsers()
        {
            return dbSet.ToList();
        }

        public UserMaster? GetUserMasterByUserId(string userId)
        {
            return this.dbSet.FirstOrDefault(x => x.UserId == userId);
        }

        public UserMaster GetUserMasterByUserIdAndPassword(string userId, string password)
        {
            return this.dbSet.FirstOrDefault(x => x.UserId == userId && x.Password == password)!;
        }

        public void AddUser(UserMaster user)
        {
            this.dbSet.Add(user);
        }

        public void UpdateUser(UserMaster user)
        {
            this.dbSet.Update(user);
        }

        public void DeleteUser(UserMaster user)
        {
            this.dbSet.Remove(user);
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
