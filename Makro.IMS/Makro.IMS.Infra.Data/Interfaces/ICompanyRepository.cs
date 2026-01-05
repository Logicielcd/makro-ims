using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface ICompanyRepository : IDisposable
    {
        IEnumerable<Company> GetCompanies();    
        Company? GetCompanyByCompanyCode(string companyCode);
        
    }

}
