using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IPoLogRepository : IDisposable
    {
        IEnumerable<PoLog> GetPoLogByPoNo(string poNo);
        PoLog GetPoLogById(int id);
        bool Add(PoLog poLog);
        bool Remove(PoLog poLog);
        bool Update(PoLog poLog);

        
    }

}
