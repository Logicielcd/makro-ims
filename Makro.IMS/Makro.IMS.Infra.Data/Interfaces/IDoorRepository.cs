using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IDoorRepository : IDisposable
    {
        IEnumerable<Door> GetDoors();
        IQueryable<Door> GetDoorPaged();
        IEnumerable<Door> GetDoorsByWarehouse(string warehouseCode);
        IEnumerable<Door> GetDoorsByDoorType(string warehouseCode,string doorType);
        Door? GetDoorById(int doorId);
        void Add(Door door);
        void Update(Door door);
        void Delete(Door door);
        IEnumerable<DoorQueueDto> GetDoorQueues(string warehouseCode);
    }

}
