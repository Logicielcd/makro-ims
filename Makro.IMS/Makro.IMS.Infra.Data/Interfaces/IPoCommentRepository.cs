using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IPoCommentRepository : IDisposable
    {
        PoComment? GetPoCommentByPoNo(string poNo);
        void Add(PoComment data);
        void Update(PoComment data);
        void Delete(PoComment data);        
    }

}
