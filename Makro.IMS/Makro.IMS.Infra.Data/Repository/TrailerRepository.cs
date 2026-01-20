using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Makro.IMS.Infra.Data.Repository
{
    public class TrailerRepository : ITrailerRepository
    {
        private IMSContext context;
        private DbSet<Trailer> dbSet;

        public TrailerRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Trailers;
        }

        public IEnumerable<Trailer> GetTrailers()
        {
            return this.dbSet.ToList();

        }

        public IQueryable<Trailer> GetTrailerPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public Trailer? GetById(decimal id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public Trailer? GetByLicensePlate(string licensePlate)
        {
            return this.dbSet.FirstOrDefault(x => x.LicensePlate == licensePlate);
        }

        public void Add(Trailer data)
        {

            this.dbSet.Add(data);
        }

        public void Update(Trailer data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(Trailer data)
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
