using Makro.IMS.Services.Api.Sieve;
using Makro.IMS.Services.Api.Sieve.Mappings.Master;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;


namespace Makro.IMS.Services.Api.Sieve
{
    public class ApplicationSieveIListProcessor : SieveIListProcessor
    {
        public ApplicationSieveIListProcessor(
            IOptions<SieveOptions> options,
            ISieveCustomFilterMethods customFilterMethods)
            : base(options, customFilterMethods)
        {
        }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            
            mapper.ApplyMapping(new BookingHeaderSieveMapping());
            mapper.ApplyMapping(new WarehouseCapacitySieveMapping());
            mapper.ApplyMapping(new WarehouseOperationCapacitySieveMapping());
            mapper.ApplyMapping(new DoorSieveMapping());
            mapper.ApplyMapping(new WarehouseSieveMapping());
            mapper.ApplyMapping(new SupplierGroupSieveMapping());
            mapper.ApplyMapping(new SupplierSieveMapping());
            mapper.ApplyMapping(new OperationTimeSieveMapping());
            mapper.ApplyMapping(new OperationSieveMapping());
            mapper.ApplyMapping(new TrailerSieveMapping());
            mapper.ApplyMapping(new ShuntSieveMapping());
            mapper.ApplyMapping(new YardSieveMapping());
            mapper.ApplyMapping(new PoMonitorSieveMapping());


            return mapper;
        }
    }
}
