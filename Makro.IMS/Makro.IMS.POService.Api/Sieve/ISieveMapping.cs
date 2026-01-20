using Sieve.Services;

namespace Makro.IMS.POServices.Api.Sieve
{
    public interface ISieveMapping<T> where T : class
    {
        void ConfigureMap(SievePropertyMapper mapper);
    }
}
