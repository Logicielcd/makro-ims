using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Makro.IMS.POServices.Api.Sieve
{
    public class ApplicationSieveProcessor : SieveProcessor
    {
        public ApplicationSieveProcessor(
            IOptions<SieveOptions> options,
            ISieveCustomFilterMethods customFilterMethods)
            : base(options, customFilterMethods)
        {
        }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            
            return mapper;
        }
    }
}
