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
    public class SupplierRepository : ISupplierRepository

    {
        private IMSContext context;
        private DbSet<Supplier> dbSet;

        public SupplierRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Suppliers;
        }


        public IQueryable<Supplier> GetSupplierPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public IEnumerable<Supplier>? GetSuppliers()
        {
            return this.dbSet.ToList();
        }

        public Supplier? GetSupplierBySupCode(string supCode)
        {
            return this.dbSet.FirstOrDefault(x => x.SupCode == supCode);
        }

        public IEnumerable<Supplier>? GetSupplierBySupGroupId(int supGroupId)
        {
            return this.dbSet.Where(x => x.InternalGroupId == supGroupId).ToList();
        }

        public IEnumerable<Supplier>? GetSuppliersSameSubGroup(string supCode)
        {
            var supGroupId = this.dbSet.Where(x => x.SupCode == supCode).Select(x => x.InternalSupGroupId).FirstOrDefault();

            return this.dbSet.Where(x => x.InternalSupGroupId!.Value == supGroupId!.Value).ToList();

        }
        public void Update(Supplier supplier)
        {
            this.dbSet.Update(supplier);
        }

        public void Add(Supplier supplier)
        {
            this.dbSet.Add(supplier);
        }

        public void Delete(Supplier supplier)
        {
            this.dbSet.Remove(supplier);
        }

        public int GetKey()
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);


            var keyId = this.context.Set<KeyId>()
                .FromSqlRaw("BEGIN PK_SUPPLIER (:V_CURSOR);  END;", new object[] { vCursor })
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
