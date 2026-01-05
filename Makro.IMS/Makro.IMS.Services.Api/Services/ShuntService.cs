using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Sieve;
using Sieve.Models;
using Sieve.Services;

namespace Makro.IMS.Services.Api.Services
{
    public class ShuntService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public ShuntService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public ShuntService(ISieveProcessor sieveProcessor)
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<Shunt>> GetShunts()
        {
            return await Task.Run<List<Shunt>>(() => unitOfWork.ShuntRepository.GetShunts().ToList());
        }

        public async Task<PagedResult<Shunt>> GetShuntPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.ShuntRepository.GetShuntPaged();
            return await _sieveProcessor.GetPagedAsync<Shunt>(queryable, sieveModel);
        }

        public async Task<Shunt?> GetById(decimal Id)
        {
            return await Task.Run<Shunt?>(() => unitOfWork.ShuntRepository.GetById(Id));
        }

        public async Task<List<Shunt>> GetByStatus(string status)
        {
            return await Task.Run<List<Shunt>>(() => unitOfWork.ShuntRepository.GetByStatus(status).ToList());
        }

        public async Task<bool> Add(Shunt shunt, string user)
        {
            Trailer check_trailer = unitOfWork.TrailerRepository.GetByLicensePlate(shunt.LicensePlate);
            if (check_trailer != null)
            {
                throw new Exception("Trailer License already existing");
            }
            if (shunt.LocationId != null)
            {
                Yard check_location = unitOfWork.YardRepository.GetById((int)(shunt.LocationId ?? 0));
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
            }
            shunt.CreateDate = DateTime.UtcNow;
            shunt.UserStamp = user;
            await Task.Run(() => unitOfWork.ShuntRepository.Add(shunt));
            unitOfWork.Save();


            return true;
        }

        public async Task<bool> Update(Shunt shunt, string user)
        {
            Shunt check_shunt = unitOfWork.ShuntRepository.GetById(shunt.Id);
            if (check_shunt == null)
            {
                throw new Exception("Shunt not found");
            }
            if (shunt.LocationId != null)
            {
                Yard check_location = unitOfWork.YardRepository.GetById((int)(shunt.LocationId ?? 0));
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
            }
            check_shunt.LicensePlate = shunt.LicensePlate;
            check_shunt.DriverName = shunt.DriverName;
            check_shunt.TelNo = shunt.TelNo;
            check_shunt.LocationId = shunt.LocationId;
            check_shunt.Status = shunt.Status;
            check_shunt.UserStamp = user;
            check_shunt.ModDate = DateTime.UtcNow;
            await Task.Run(() => unitOfWork.ShuntRepository.Update(check_shunt));
            unitOfWork.Save();

            return true;
        }

        public async Task<bool> Delete(decimal Id)
        {
            Shunt shunt = unitOfWork.ShuntRepository.GetById(Id);
            if (shunt != null)
            {
                await Task.Run(() => unitOfWork.ShuntRepository.Delete(shunt));
                unitOfWork.Save();
            }
            else
            {
                throw new Exception("Shunt not found");
            }
            return true;
        }
    }
}
