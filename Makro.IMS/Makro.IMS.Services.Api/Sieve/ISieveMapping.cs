using Sieve.Services;

namespace Makro.IMS.Services.Api.Sieve
{
    public interface ISieveMapping<T> where T : class
    {
        void ConfigureMap(SievePropertyMapper mapper);
    }
}
