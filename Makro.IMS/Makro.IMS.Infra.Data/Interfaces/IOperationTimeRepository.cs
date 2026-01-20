using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IOperationTimeRepository : IDisposable
    {
        IEnumerable<OperationTime> GetOperationTimes();
        IQueryable<OperationTime> GetOperationTimesPaged();
        IEnumerable<OperationTime> GetOperationTimesByWarehouse(string warehouseCode);
        IEnumerable<OperationTime> GetOperationTimesByWarehouseByOperationType(string warehouseCode,string operationName); 
        OperationTime? GetOperationById(decimal id);
        void Add(OperationTime data);
        void Update(OperationTime data);
        void Delete(OperationTime data);
    }

}
