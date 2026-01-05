using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class PoCommentRepository : IPoCommentRepository
    {
        private IMSContext context;
        private DbSet<PoComment> dbSet;

        public PoCommentRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.PoComments;
        }

        public PoComment? GetPoCommentByPoNo(string poNo)
        {
            return this.dbSet.FirstOrDefault(x=>x.Po == poNo);
        }

        public void Add(PoComment data)
        {
            this.dbSet.Add(data);
        }

        public void Update(PoComment data)
        {
            this.dbSet.Update(data);
        }

        public void Delete(PoComment data)
        {
            this.dbSet.Remove(data);
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
