using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface ITruckCapRepository : IDisposable
    {
        IEnumerable<TruckCap> GetTruckCaps();
        void Update(TruckCap truckCap);
        void Add(TruckCap truckCap);

    }

}
