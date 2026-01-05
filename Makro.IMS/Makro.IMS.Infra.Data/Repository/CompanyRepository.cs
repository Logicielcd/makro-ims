using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private IMSContext context;
        private DbSet<Company> dbSet;

        public CompanyRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Companies;
        }
        public IEnumerable<Company> GetCompanies()
        {
            return this.dbSet.ToList();
        }

        public Company? GetCompanyByCompanyCode(string companyCode)
        {
            return this.dbSet.FirstOrDefault(x => x.CompanyCode == companyCode);
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
