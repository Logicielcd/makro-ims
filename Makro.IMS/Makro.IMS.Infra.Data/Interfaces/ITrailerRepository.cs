using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Infra.Data.Interfaces
{
    public interface ITrailerRepository : IDisposable
    {
        IEnumerable<Trailer> GetTrailers();
        IQueryable<Trailer> GetTrailerPaged();
        Trailer? GetById(decimal id);
        void Add(Trailer data);
        void Update(Trailer data);
        void Delete(Trailer data);
    }
}
