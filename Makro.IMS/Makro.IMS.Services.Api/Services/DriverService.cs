using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;
using System.Reflection.Emit;

namespace Makro.IMS.Services.Api.Services
{
    public class DriverService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public DriverService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<QueueManageDto>> GetGatePassByTelNo(string telNo)
        {
            List<QueueManageDto> result;

            result = unitOfWork.BookingHeaderRepository.GetGatePassByTelNo(telNo);

            return result;
        }

        public async Task<List<DoorQueueDto>> GetDoorQueue(string warehouseCode)
        {
            
            var result = unitOfWork.DoorRepository.GetDoorQueues(warehouseCode);

            return result.ToList();
        }

        public async Task<List<TruckCheckIn>> GetGatePassByLicensePlate(string licensePlate)
        {
            List<TruckCheckIn> result;

            result = unitOfWork.BookingHeaderRepository.GetGatePassByLicensePlate(licensePlate);

            return result;
        }

        public async Task<List<TruckCheckIn>> GetGatePassCheckoutByLicensePlate(string licensePlate)
        {
            List<TruckCheckIn> result;

            result = unitOfWork.BookingHeaderRepository.GetTruckCheckoutByLicensePlate(licensePlate);

            return result;
        }
    }
}
