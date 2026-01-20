using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IUserMasterRepository : IDisposable
    {
        IEnumerable<UserMaster> GetUsers();    
        UserMaster GetUserMasterByUserId(string userId);
        UserMaster GetUserMasterByUserIdAndPassword(string userId, string password);

        void AddUser(UserMaster user);
        void UpdateUser(UserMaster user);
        void DeleteUser(UserMaster user);
    }

}
