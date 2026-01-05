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
    public class OperationTimeService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public OperationTimeService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public OperationTimeService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<OperationTime>> GetOperationTimes()
        {
            return await Task.Run<List<OperationTime>>(() => unitOfWork.OperationTimeRepository.GetOperationTimes().ToList());
        }

        public async Task<PagedResult<OperationTime>> GetOperationTimePaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.OperationTimeRepository.GetOperationTimesPaged();
            return await _sieveProcessor.GetPagedAsync<OperationTime>(queryable, sieveModel);
        }

        public async Task<List<OperationTime>> GetOperationTimesByWarehouse(string warehouseCode)
        {
            return await Task.Run<List<OperationTime>>(() => unitOfWork.OperationTimeRepository.GetOperationTimesByWarehouse(warehouseCode).ToList());
        }

        public async Task<List<OperationTime>> GetOperationTimesByWarehouseAndOperationType(string warehouseCode,string operationType)
        {
            return await Task.Run<List<OperationTime>>(() => unitOfWork.OperationTimeRepository.GetOperationTimesByWarehouseByOperationType(warehouseCode,operationType).ToList());
        }


        public async Task<OperationTime?> GetById(decimal id)
        {
            return await Task.Run<OperationTime?>(() => unitOfWork.OperationTimeRepository.GetOperationById(id));
        }

        public async Task<bool> Add(OperationTime operationTime)
        {            
            var ops = unitOfWork.OperationTimeRepository.GetOperationTimesByWarehouseByOperationType(operationTime.WarehouseCode, operationTime.OperationType);

            operationTime.StartTime = operationTime.StartTime.ToLocalTime();
            operationTime.EndTime = operationTime.EndTime.ToLocalTime();

            var opsStartTime = Convert.ToInt32(operationTime.StartTime.Hour.ToString().PadLeft(2, '0') + operationTime.StartTime.Minute.ToString().PadLeft(2,'0'));
            var opsEndTime = Convert.ToInt32(operationTime.EndTime.Hour.ToString().PadLeft(2, '0') + operationTime.EndTime.Minute.ToString().PadLeft(2, '0'));

            var dup = ops.Count(x=> (opsStartTime >= Convert.ToInt32(x.StartTime.Hour.ToString().PadLeft(2,'0') + x.StartTime.Minute.ToString().ToString().PadLeft(2,'0')) &&
            opsStartTime <= Convert.ToInt32(x.EndTime.Hour.ToString().PadLeft(2, '0') + x.EndTime.Minute.ToString().ToString().PadLeft(2, '0'))) ||
            (opsEndTime >= Convert.ToInt32(x.StartTime.Hour.ToString().PadLeft(2, '0') + x.StartTime.Minute.ToString().ToString().PadLeft(2, '0')) &&
            opsEndTime <= Convert.ToInt32(x.EndTime.Hour.ToString().PadLeft(2, '0') + x.EndTime.Minute.ToString().ToString().PadLeft(2, '0')))
            );

            if(dup > 0)
            {
                throw new Exception("Cannot select duplicate time slot");
            }

            unitOfWork.OperationTimeRepository.Add(operationTime);

            unitOfWork.Save();

            return true;
        }

        public async Task<bool> Delete(decimal id)
        {
            var operationTime = await Task.Run<OperationTime?>(() => unitOfWork.OperationTimeRepository.GetOperationById(id));

            unitOfWork.OperationTimeRepository.Delete(operationTime);

            unitOfWork.Save();

            return true;
        }



    }
}
