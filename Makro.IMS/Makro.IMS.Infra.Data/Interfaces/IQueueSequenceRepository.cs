using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IQueueSequenceRepository : IDisposable
    {
        QueueSequence? GetQueueSeq(string warehouseCode,string operationType);
        void Add(QueueSequence queue);
        void Update(QueueSequence queue);
        
    }

}
