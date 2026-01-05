using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IUserWarehouseRepository : IDisposable
    {        
        IEnumerable<UserWarehouse> GetUserWarehouseByUserId(string userId);
        void AddUserWarehouse(UserWarehouse userWarehouse);
        void AddRangeUserWarehouse(List<UserWarehouse> userWarehouses);
        void DeleteUserWarehouse(UserWarehouse userWarehouse);
    }

}
