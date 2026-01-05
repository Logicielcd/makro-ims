using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class SupplierGroupRepository : ISupplierGroupRepository
    {
        private IMSContext context;
        private DbSet<SupplierGroup> dbSet;

        public SupplierGroupRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.SupplierGroups;
        }


        public IQueryable<SupplierGroup> GetSupplierGroupPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public SupplierGroup? GetSupplierGroupById(int supGroupId)
        {
            return this.dbSet.FirstOrDefault(x => x.InternalSupGroupId == supGroupId);
        }

        public IEnumerable<SupplierGroup> GetAll()
        {
            return this.dbSet.ToList();
        }

        public void UpdateSupplierContact(SupplierGroup supplierGroup)
        {
            this.dbSet.Update(supplierGroup);            
        }

        public void Update(SupplierGroup supplierGroup)
        {
            this.dbSet.Update(supplierGroup);
        }

        public void Add(SupplierGroup supplierGroup)
        {
            this.dbSet.Add(supplierGroup);
        }

        public void Remove(SupplierGroup supplierGroup)
        {
            this.dbSet.Remove(supplierGroup);
        }


        public int GetKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_SUPPLIERGROUP (:V_CURSOR);  END;", new object[] { vCursor })
                .ToList();

            return keyId[0].Nextval;
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
