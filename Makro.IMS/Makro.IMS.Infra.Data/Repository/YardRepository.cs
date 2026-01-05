using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Makro.IMS.Infra.Data.Repository
{
    public class YardRepository : IYardRepository
    {
        private IMSContext context;
        private DbSet<Yard> dbSet;

        public YardRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Yards;
        }
        public IEnumerable<Yard> GetYards()
        {
            return this.dbSet.ToList();
        }

        public IQueryable<Yard> GetYardPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public Yard? GetById(decimal id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public Yard? GetByYardNo(string yardNo)
        {
            return this.dbSet.FirstOrDefault(x => x.YardNo == yardNo);
        }

        public IQueryable<Yard> GetByYardType(string yardType)
        {
            return this.dbSet.Where(x => x.YardType == yardType);
        }

        public void Add(Yard data)
        {

            this.dbSet.Add(data);
        }

        public void Update(Yard data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(Yard data)
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
