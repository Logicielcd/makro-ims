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
    public class TruckMasterService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;        

        public TruckMasterService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public async Task<List<TruckMaster>> GetTrucks()
        {
            return await Task.Run<List<TruckMaster>>(() => unitOfWork.TruckMasterRepository.GetTrucks().ToList());
        }

        public async Task<List<TruckCap>> GetTruckCapAll()
        {
            return await Task.Run<List<TruckCap>>(() => unitOfWork.TruckCapRepository.GetTruckCaps().ToList());
        }

        public async Task<List<TruckCap>> GetTruckCaps(string whseCode)
        {
            return await Task.Run<List<TruckCap>>(() => unitOfWork.TruckCapRepository.GetTruckCaps().ToList().Where(x=>x.WarehouseCode == whseCode).ToList());
        }
        public async Task<List<TruckRule>> GetTruckRules()
        {
            return await Task.Run<List<TruckRule>>(() => unitOfWork.TruckRuleRepository.GetTruckRules().ToList());
        }

    }
}
