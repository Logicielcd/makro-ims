using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IUserSupplierGroupRepository : IDisposable
    {
        IEnumerable<UserSupplierGroup> GetUserSupplierByUserId(string userId);    
        void AddUserSupplierGroup(UserSupplierGroup userSupplierGroup);
        void AddRangeUserSupplierGroup(List<UserSupplierGroup> userSupplierGroup);
        void DeleteUserSupplierGroup(UserSupplierGroup userSupplierGroup);

    }

}
