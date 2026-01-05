using System.Linq;
using Sieve.Models;

namespace Sieve.Services
{
    public interface ISieveIListProcessor : ISieveIListProcessor<SieveModel, FilterTerm, SortTerm>
    {

    }

    public interface ISieveIListProcessor<TFilterTerm, TSortTerm> : ISieveIListProcessor<SieveModel<TFilterTerm, TSortTerm>, TFilterTerm, TSortTerm>
        where TFilterTerm : IFilterTerm, new()
        where TSortTerm : ISortTerm, new()
    {

    }

    public interface ISieveIListProcessor<TSieveModel, TFilterTerm, TSortTerm>
        where TSieveModel : class, ISieveModel<TFilterTerm, TSortTerm>
        where TFilterTerm : IFilterTerm, new()
        where TSortTerm : ISortTerm, new()

    {
        List<TEntity> Apply<TEntity>(
            TSieveModel model,
            List<TEntity> source,
            object[] dataForCustomMethods = null,
            bool applyFiltering = true,
            bool applySorting = true,
            bool applyPagination = true);
    }
}