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
    public class EstTimeRepository : IEstTimeRepository
    {
        private IMSContext context;
        private DbSet<EstTime> dbSet;

        public EstTimeRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.EstTimes;
        }
        public IEnumerable<EstTime> GetEstTimes()
        {
            return this.dbSet.ToList();
        }
        public IEnumerable<EstTime> GetEstTimeBySupGroup(int supGroupId)
        {
            return this.dbSet.Where(x=>x.InternalSupGroupId == supGroupId).ToList();
        }

        public EstTime GetEstTimeByUd(int estTimeId)
        {
            return this.dbSet.FirstOrDefault(x => x.InternalEstId == estTimeId);
        }

        public void CreateEstimateTime(int supGroupId)
        {
            
            var vSupGroupId = new OracleParameter("v_INTERNAL_SUP_GROUP_ID", OracleDbType.Int32, ParameterDirection.Input);
            vSupGroupId.Value = supGroupId;


            var poList = this.context.Database.ExecuteSqlRaw("BEGIN EST_TIME_SET_AUTO (:v_INTERNAL_SUP_GROUP_ID);  END;", new object[] { vSupGroupId });
                

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

        public void Add(EstTime estTime)
        {
            this.dbSet.Add(estTime);
        }

        public void Delete(EstTime estTime)
        {
            this.dbSet.Remove(estTime);
        }

    }
}
