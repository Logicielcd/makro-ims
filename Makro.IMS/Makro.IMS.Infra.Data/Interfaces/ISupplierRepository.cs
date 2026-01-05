using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface ISupplierRepository : IDisposable
    {
        IEnumerable<Supplier>? GetSuppliers();
        IQueryable<Supplier> GetSupplierPaged();
        Supplier? GetSupplierBySupCode(string supCode);
        IEnumerable<Supplier>? GetSuppliersSameSubGroup(string supCode);
        IEnumerable<Supplier>? GetSupplierBySupGroupId(int supGroupId);

    }

}
