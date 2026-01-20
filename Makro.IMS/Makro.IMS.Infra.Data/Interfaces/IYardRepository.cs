using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Infra.Data.Interfaces
{
    public interface IYardRepository : IDisposable
    {
        IEnumerable<Yard> GetYards();
        IQueryable<Yard> GetYardPaged();
        Yard? GetById(decimal id);
        void Add(Yard data);
        void Update(Yard data);
        void Delete(Yard data);
    }
}
