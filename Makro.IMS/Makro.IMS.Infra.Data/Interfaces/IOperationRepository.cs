using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IOperationRepository : IDisposable
    {
        IEnumerable<Operation> GetOperations();
        IQueryable<Operation> GetOperationPaged();
        IEnumerable<Operation> GetOperationsByWarehouse(string warehouseCode);
        Operation? GetOperationByNameAndWhse(string operationName,string warehouseCode);
        void Add(Operation data);
        void Update(Operation data);
        void Delete(Operation data);
    }

}
