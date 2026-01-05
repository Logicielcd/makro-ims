using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;

namespace Makro.IMS.Services.Api.Services
{
    public class YardService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public YardService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public YardService(ISieveProcessor sieveProcessor)
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<Yard>> GetYards()
        {
            return await Task.Run<List<Yard>>(() => unitOfWork.YardRepository.GetYards().ToList());
        }

        public async Task<List<YardMonitorDto>> GetYardMonitors()
        {
            var yards = await Task.Run<List<Yard>>(() => unitOfWork.YardRepository.GetYards().ToList());
            var trailers = await Task.Run<List<Trailer>>(() => unitOfWork.TrailerRepository.GetTrailers().ToList());

            // Create lookup dictionary for trailers, handling duplicates by taking the first trailer for each location
            var trailerLookup = trailers
                .Where(t => t.LocationId != null)
                .GroupBy(t => t.LocationId)
                .ToDictionary(g => g.Key, g => g.First());

            return yards.Select(yard =>
            {
                // Try to get trailer for this yard from the lookup dictionary
                trailerLookup.TryGetValue(yard.Id, out Trailer matchingTrailer);
                return new YardMonitorDto
                {
                    YardId = yard.Id,
                    YardNo = yard.YardNo,
                    YardZone = yard.YardZone,
                    YardStatus = yard.Status,
                    TrailerId = matchingTrailer?.Id,
                    TrailerLicensePlate = matchingTrailer?.LicensePlate,
                    TrailerStatus = matchingTrailer?.Status
                };
            }).OrderBy(x => x.YardNo).ToList();
        }

        public async Task<PagedResult<Yard>> GetYardPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.YardRepository.GetYardPaged();
            return await _sieveProcessor.GetPagedAsync<Yard>(queryable, sieveModel);
        }

        public async Task<Yard?> GetById(decimal Id)
        {
            return await Task.Run<Yard?>(() => unitOfWork.YardRepository.GetById(Id));
        }

        public async Task<List<Yard>> GetByYardType(string yardType)
        {
            return await Task.Run<List<Yard>>(() => unitOfWork.YardRepository.GetByYardType(yardType).ToList());
        }

        public async Task<bool> Add(Yard yard, string user)
        {
            Yard check_yard = unitOfWork.YardRepository.GetByYardNo(yard.YardNo);
            if (check_yard != null)
            {
                throw new Exception("Yard already existing");
            }
            else
            {
                yard.CreateDate = DateTime.UtcNow;
                yard.UserStamp = user;
                await Task.Run(() => unitOfWork.YardRepository.Add(yard));
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> Update(Yard yard, string user)
        {
            Yard check_yard = unitOfWork.YardRepository.GetById(yard.Id);
            if (check_yard == null)
            {
                throw new Exception("Yard not found");
            }
            else
            {
                check_yard.YardNo = yard.YardNo;
                check_yard.YardType = yard.YardType;
                check_yard.YardZone = yard.YardZone;
                check_yard.TrailerType = yard.TrailerType;
                check_yard.LocationNo = yard.LocationNo;
                check_yard.Status = yard.Status;
                check_yard.UserStamp = user;
                check_yard.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.YardRepository.Update(check_yard));
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Delete(decimal Id)
        {
            Yard yard = unitOfWork.YardRepository.GetById(Id);

            if (yard != null)
            {
                await Task.Run(() => unitOfWork.YardRepository.Delete(yard));
                unitOfWork.Save();
            }
            else
            {
                throw new Exception("Yard not found");
            }
            return true;
        }


    }
}
