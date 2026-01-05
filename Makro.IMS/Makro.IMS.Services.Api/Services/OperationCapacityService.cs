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
    public class OperationCapacityService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public OperationCapacityService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public OperationCapacityService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }
        
        public async Task<List<OperationCapacity>> GetByNameAndWhse(string operationName,string warehouseCode)
        {
            return await Task.Run<List<OperationCapacity>>(() => unitOfWork.OperationCapacityRepository.GetOperationByNameAndWhse(operationName,warehouseCode));
        }

        public async Task<bool> Add(OperationCapacity data)
        {
            // Check duplicate
            data.Time = data.Time.Value.ToLocalTime();
            var ope = unitOfWork.OperationCapacityRepository.GetOperationByNameAndWhseAndTime(data.OperationType, data.WarehouseCode,data.Time.Value);            

            if (ope != null)
            {
                ope.MonCap = data.MonCap;
                ope.TueCap = data.TueCap;
                ope.WedCap = data.WedCap;
                ope.ThuCap = data.ThuCap;
                ope.FriCap = data.FriCap;
                ope.SatCap = data.SatCap;
                ope.SunCap = data.SunCap;
                ope.MonTruck = data.MonTruck;
                ope.TueTruck = data.TueTruck;
                ope.WedTruck = data.WedTruck;
                ope.ThuTruck = data.ThuTruck;
                ope.FriTruck = data.FriTruck;
                ope.SatTruck = data.SatTruck;
                ope.SunTruck = data.SunTruck;

                await Task.Run(() => unitOfWork.OperationCapacityRepository.Update(ope));
                unitOfWork.Save();
            }
            else
            {                
                await Task.Run(() => unitOfWork.OperationCapacityRepository.Add(data));
                unitOfWork.Save();
            }
            return true;
        }


        //public async Task<bool> Delete(Operation data)
        //{
         

        //    // Check duplicate
        //    var operationUpdate = unitOfWork.OperationRepository.GetOperationByNameAndWhse(data.OperationName, data.WarehouseCode);

        //    if (operationUpdate == null)
        //    {
        //        throw new Exception("Operation not found.");
        //    }
        //    else
        //    {                
        //        await Task.Run(() => unitOfWork.OperationRepository.Delete(operationUpdate));
        //        unitOfWork.Save();
        //    }
        //    return true;
        //}
    }
}
