using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IOperationCapacityRepository : IDisposable
    {        
        List<OperationCapacity> GetOperationByNameAndWhse(string operationName,string warehouseCode);
        OperationCapacity? GetOperationByNameAndWhseAndTime(string operationName, string warehouseCode,DateTime time);
        void Add(OperationCapacity data);
        void Update(OperationCapacity data);
        void Delete(OperationCapacity data);
    }

}
