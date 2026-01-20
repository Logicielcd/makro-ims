using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IPoCheckInOutRepository : IDisposable
    {
        IEnumerable<PoCheckInOut> GetPoCheckInOuts();
        IEnumerable<PoCheckInOut> GetPoCheckInOutsByInternalTruckCheckInId(int id);
        IEnumerable<PoCheckInOut> GetPoCheckOutsByInternalTruckCheckInId(int id);
        PoCheckInOut? GetPoCheckInOut(int id);
        PoCheckInOut? GetPoCheckIn(string poNbr);
        PoCheckInOut? GetPoCheckOut(string poNbr);
        void Add(PoCheckInOut poCheckInOut);
        void Update(PoCheckInOut poCheckInOut);
        void Delete(PoCheckInOut poCheckInOut);
    }

}
