using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IOperationFixSlotRepository : IDisposable
    {        
        IEnumerable<OperationFixSlot> GetOperationFixSlotByWhseAndOpType(string warehouseCode, string operationName);
        IEnumerable<OperationFixSlot> GetOperationFixSlotByWhseAndOpTypeAndSupGroup(string warehouseCode,string operationName,decimal supGroupId);
        OperationFixSlot? GetOperationById(decimal id);
        void Add(OperationFixSlot data);
        void Update(OperationFixSlot data);
        void Delete(OperationFixSlot data);
    }

}
