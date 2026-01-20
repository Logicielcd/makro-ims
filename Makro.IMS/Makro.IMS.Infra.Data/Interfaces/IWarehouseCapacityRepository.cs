using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IWarehouseCapacityRepository : IDisposable
    {
        IEnumerable<WarehouseCapacity> GetWarehousesCapacity();
        IQueryable<WarehouseCapacity> GetWarehousesCapacityPaged();
        WarehouseCapacity? GetWarehouseCapacirtyById(int id);

        void Add(WarehouseCapacity warehouseCapacity);
        void Update(WarehouseCapacity warehouseCapacity);
        void Remove(WarehouseCapacity warehouseCapacity);

        IEnumerable<WarehouseCapacity> GetByWarehouse(string warehouse);
        WarehouseCapacity GetByWarehouseBookingDate(string warehouse, DateTime bookingDate);

    }

}
