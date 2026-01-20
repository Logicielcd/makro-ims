using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Makro.IMS.Infra.Data.Repository
{
    public class ShuntRepository : IShuntRepository
    {
        private IMSContext context;
        private DbSet<Shunt> dbSet;

        public ShuntRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Shunts;
        }

        public IEnumerable<Shunt> GetShunts()
        {
            return this.dbSet.ToList();
        }

        public IQueryable<Shunt> GetShuntPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public Shunt? GetById(decimal id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Shunt> GetByStatus(string status)
        {
            return this.dbSet.Where(x => x.Status == status).ToList();
        }

        public void Add(Shunt data)
        {

            this.dbSet.Add(data);
        }

        public void Update(Shunt data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(Shunt data)
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
