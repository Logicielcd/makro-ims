using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using System.Linq;

namespace Makro.IMS.Services.Api.Services
{
    public class EstTimeService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;        

        public EstTimeService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public async Task<List<EstTime>> GetEstTimes()
        {
            return await Task.Run<List<EstTime>>(() => unitOfWork.EstTimeRepository.GetEstTimes().ToList());
        }

        public async Task<List<EstTime>> GetEstTimeBySupGroupId(int supGroupId)
        {
            return await Task.Run<List<EstTime>>(() => unitOfWork.EstTimeRepository.GetEstTimeBySupGroup(supGroupId).ToList());
        }

        public async Task<bool> AddEst(EstTime est)
        {
            // Check duplicate
            var ests = unitOfWork.EstTimeRepository.GetEstTimeBySupGroup(est.InternalSupGroupId.Value).ToList();

            var cntEst = ests.Count(x=>x.InternalTruckId == est.InternalTruckId && x.OperationType == est.OperationType && x.WarehouseCode == est.WarehouseCode);
            if (cntEst > 0)
            {
                throw new Exception("Estimate time already existing.");
            }
            else
            {
                var estId = unitOfWork.EstTimeRepository.GetEstTimes().Max(x=>x.InternalEstId);

                unitOfWork.Save();

                est.InternalEstId = estId+1;
                await Task.Run(() => unitOfWork.EstTimeRepository.Add(est));

                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Delete(int estTimeId)
        {
            // Check duplicate
            var est = unitOfWork.EstTimeRepository.GetEstTimeByUd(estTimeId);

            if (est == null)
            {
                throw new Exception("Est Time not found.");
            }
            else
            {
                await Task.Run(() => unitOfWork.EstTimeRepository.Delete(est));                
                unitOfWork.Save();
            }
            return true;
        }

    }
}
