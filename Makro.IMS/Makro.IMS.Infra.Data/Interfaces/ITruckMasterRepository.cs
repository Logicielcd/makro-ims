using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface ITruckMasterRepository : IDisposable
    {
        IEnumerable<TruckMaster> GetTrucks();
    }

}
