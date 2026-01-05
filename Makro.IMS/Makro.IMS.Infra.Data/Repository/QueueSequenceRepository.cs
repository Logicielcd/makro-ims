using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class QueueSequenceRepository : IQueueSequenceRepository
    {
        private IMSContext context;
        private DbSet<QueueSequence> dbSet;

        public QueueSequenceRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.QueueSequences;
        }

        public QueueSequence? GetQueueSeq(string warehouseCode, string operationType)
        {
            return this.dbSet.FirstOrDefault(x => x.WarehouseCode == warehouseCode && x.OperationType == operationType);
        }

        public void Add(QueueSequence queue)
        {
            this.dbSet.Add(queue);
        }

        public void Update(QueueSequence queue)
        {
            this.dbSet.Update(queue);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
