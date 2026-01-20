using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;

namespace Makro.IMS.Services.Api.Services
{
    public class TrailerService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public TrailerService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public TrailerService(ISieveProcessor sieveProcessor)
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<TrailerDto>> GetTrailers()
        {
            var trailers = unitOfWork.TrailerRepository.GetTrailers();
            var jobs = unitOfWork.JobRepository.GetAllJobs();
            var yards = unitOfWork.YardRepository.GetYards();
            var activeJobs = jobs.Where(j => j.Status == "New" || j.Status == "Assigned" || j.Status == "In-Process" ||
                                            j.Status == "On-Dock" || j.Status == "Loading" || j.Status == "Loaded")
                                .GroupBy(j => j.TrailerId)
                                .ToDictionary(g => g.Key, g => g.FirstOrDefault());
            var yardLookup = yards.ToDictionary(y => y.Id);

            return trailers.Select(t =>
            {
                var yard = t.LocationId.HasValue ? yardLookup.GetValueOrDefault(t.LocationId.Value) : null;
                var job = activeJobs.GetValueOrDefault(t.Id);
                string jobStatus = job != null ? "On-Job" : null;

                // Initialize jobDockStatus as null
                string jobDockStatus = null;
                if (job != null)
                {
                    // Check for Change Trailer condition only if trailer is Empty
                    if (yard?.YardType == "Dock" &&
                        (job.Status == "New" || job.Status == "Assigned" || job.Status == "In-Process") &&
                        (t.Status == "On-Dock")) // Added condition to check if trailer is empty
                    {
                        jobDockStatus = "Change Trailer";
                    }
                    // Only set jobDockStatus for Loading or Loaded status
                    else if (job.Status == "Loading" || job.Status == "Loaded")
                    {
                        jobDockStatus = job.Status;
                    }
                    // All other statuses will remain null
                }

                return new TrailerDto
                {
                    Id = t.Id,
                    LicensePlate = t.LicensePlate,
                    TrailerType = t.TrailerType,
                    TrailerGroup = t.TrailerGroup,
                    TrailerSize = t.TrailerSize,
                    Status = t.Status,
                    LocationId = t.LocationId,
                    LocationNo = yard?.YardNo,
                    LocationType = yard?.YardType,
                    LocationZone = yard?.YardZone,
                    CreateDate = t.CreateDate,
                    ModDate = t.ModDate,
                    UserStamp = t.UserStamp,
                    JobStatusOriginal = job?.Status,
                    JobStatus = jobStatus,
                    JobDockStatus = jobDockStatus
                };
            }).ToList();
        }

        public async Task<PagedResult<Trailer>> GetTrailerPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.TrailerRepository.GetTrailerPaged();
            return await _sieveProcessor.GetPagedAsync<Trailer>(queryable, sieveModel);
        }

        public async Task<Trailer?> GetById(decimal Id)
        {
            return await Task.Run<Trailer?>(() => unitOfWork.TrailerRepository.GetById(Id));
        }

        public async Task<bool> Add(Trailer trailer, string user)
        {
            Trailer check_trailer = unitOfWork.TrailerRepository.GetByLicensePlate(trailer.LicensePlate);
            if (check_trailer != null)
            {
                throw new Exception("Trailer License already existing");
            }
            Yard check_location = unitOfWork.YardRepository.GetById(trailer.LocationId ?? 0);
            if (check_location == null)
            {
                throw new Exception("Location not found");
            }
            //if (check_location.Status == "Empty")
            if (check_location.Status == "Available")
            {
                check_location.Status = "Not Available";
                await Task.Run(() => unitOfWork.YardRepository.Update(check_location));
                unitOfWork.Save();
            }
            else
            {
                trailer.CreateDate = DateTime.UtcNow;
                trailer.UserStamp = user;
                await Task.Run(() => unitOfWork.TrailerRepository.Add(trailer));
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> Update(Trailer trailer, string user)
        {
            Trailer check_trailer = unitOfWork.TrailerRepository.GetById(trailer.Id);
            if (check_trailer == null)
            {
                throw new Exception("Trailer not found");
            }
            Yard check_location = unitOfWork.YardRepository.GetById(trailer.LocationId ?? 0);
            if (check_location == null)
            {
                throw new Exception("Location not found");
            }
            //if (check_location.Status == "Empty")
            if (check_location.Status == "Available")
            {
                //check_location.Status = "Full";
                check_location.Status = "Not Available";
                await Task.Run(() => unitOfWork.YardRepository.Update(check_location));
                unitOfWork.Save();
            }
            else
            {
                check_trailer.LicensePlate = trailer.LicensePlate;
                check_trailer.TrailerType = trailer.TrailerType;
                check_trailer.TrailerGroup = trailer.TrailerGroup;
                check_trailer.TrailerSize = trailer.TrailerSize;
                check_trailer.LocationId = trailer.LocationId;
                check_trailer.Status = trailer.Status;
                check_trailer.ModDate = DateTime.UtcNow;
                check_trailer.UserStamp = user;
                await Task.Run(() => unitOfWork.TrailerRepository.Update(check_trailer));
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Delete(decimal Id)
        {
            Trailer trailer = unitOfWork.TrailerRepository.GetById(Id);

            if (trailer != null)
            {
                await Task.Run(() => unitOfWork.TrailerRepository.Delete(trailer));
                unitOfWork.Save();
            }
            else
            {
                throw new Exception("Trailer not found");
            }
            return true;
        }


    }
}
