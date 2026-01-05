using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Infra.Data.Interfaces
{
    public interface IShuntRepository : IDisposable
    {
        IEnumerable<Shunt> GetShunts();
        IQueryable<Shunt> GetShuntPaged();
        Shunt? GetById(decimal id);
        void Add(Shunt data);
        void Update(Shunt data);
        void Delete(Shunt data);
    }
}
