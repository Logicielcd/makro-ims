using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IWarehouseOperationCapacityRepository : IDisposable
    {
        IEnumerable<WarehouseOperationCapacity> GetWarehousesCapacity();
        IQueryable<WarehouseOperationCapacity> GetWarehousesCapacityPaged();
        WarehouseOperationCapacity? GetWarehouseCapacirtyById(int id);

        void Add(WarehouseOperationCapacity warehouseCapacity);
        void Update(WarehouseOperationCapacity warehouseCapacity);
        void Remove(WarehouseOperationCapacity warehouseCapacity);

        IEnumerable<WarehouseOperationCapacity> GetByWarehouse(string warehouse);
        WarehouseOperationCapacity GetByWarehouseBookingDate(string warehouse, DateTime bookingDate);

    }

}
