using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IWarehouseRepository : IDisposable
    {
        IEnumerable<Warehouse> GetWarehouses();
        IQueryable<Warehouse> GetWarehousesPaged();

        Warehouse? GetWarehouseByWhseCode(string whseCode);

        void Add(Warehouse warehouse);
        void Update(Warehouse warehouse);
        void Delete(Warehouse warehouse);
    }

}
