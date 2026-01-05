using Makro.IMS.Infra.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface ISupplierGroupRepository : IDisposable
    {    
        IEnumerable<SupplierGroup> GetAll();
        IQueryable<SupplierGroup> GetSupplierGroupPaged();
        SupplierGroup? GetSupplierGroupById(int supGroupId);
        void UpdateSupplierContact (SupplierGroup supplierGroup);
        void Update (SupplierGroup supplierGroup);
        void Add (SupplierGroup supplierGroup);
    }

}
