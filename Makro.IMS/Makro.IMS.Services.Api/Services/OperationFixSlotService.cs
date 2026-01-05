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
    public class OperationFixSlotService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public OperationFixSlotService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public OperationFixSlotService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<OperationFixSlot>> GetOperationFixSlotByWhseAndOpType(string warehouseCode,string operationType)
        {
            return await Task.Run<List<OperationFixSlot>>(() => unitOfWork.OperationFixSlotRepository.GetOperationFixSlotByWhseAndOpType(warehouseCode,operationType).ToList());
        }

        public async Task<List<OperationFixSlot>> GetOperationFixSlotByWhseAndOpTypeAndSupGroupId(string warehouseCode, string operationType,decimal supGroupId)
        {
            return await Task.Run<List<OperationFixSlot>>(() => unitOfWork.OperationFixSlotRepository.GetOperationFixSlotByWhseAndOpTypeAndSupGroup(warehouseCode, operationType,supGroupId).ToList());
        }


        public async Task<OperationFixSlot?> GetById(decimal id)
        {
            return await Task.Run<OperationFixSlot?>(() => unitOfWork.OperationFixSlotRepository.GetOperationById(id));
        }

        public async Task<bool> Add(OperationFixSlot operationFixSlot)
        {            
            var ops = unitOfWork.OperationFixSlotRepository.GetOperationFixSlotByWhseAndOpTypeAndSupGroup(operationFixSlot.WarehouseCode, operationFixSlot.OperationType,operationFixSlot.SupGroupId);

            ops = ops.Where(x => x.DaysOfWeek == operationFixSlot.DaysOfWeek);

            if(ops.Count() > 0)
            {
                ops = ops.Where(x=>x.SupGroupId == operationFixSlot.SupGroupId).ToList();
            }

            operationFixSlot.StartTime = operationFixSlot.StartTime.ToLocalTime();
            operationFixSlot.EndTime = operationFixSlot.EndTime.ToLocalTime();
            operationFixSlot.IsVip = operationFixSlot.IsVip;

            var opsStartTime = Convert.ToInt32(operationFixSlot.StartTime.Hour.ToString().PadLeft(2, '0') + operationFixSlot.StartTime.Minute.ToString().PadLeft(2, '0'));
            var opsEndTime = Convert.ToInt32(operationFixSlot.EndTime.Hour.ToString().PadLeft(2, '0') + operationFixSlot.EndTime.Minute.ToString().PadLeft(2, '0'));

            var dup = ops.Count(x => (opsStartTime >= Convert.ToInt32(x.StartTime.Hour.ToString().PadLeft(2, '0') + x.StartTime.Minute.ToString().ToString().PadLeft(2, '0')) &&
            opsStartTime <= Convert.ToInt32(x.EndTime.Hour.ToString().PadLeft(2, '0') + x.EndTime.Minute.ToString().ToString().PadLeft(2, '0'))) ||
            (opsEndTime >= Convert.ToInt32(x.StartTime.Hour.ToString().PadLeft(2, '0') + x.StartTime.Minute.ToString().ToString().PadLeft(2, '0')) &&
            opsEndTime <= Convert.ToInt32(x.EndTime.Hour.ToString().PadLeft(2, '0') + x.EndTime.Minute.ToString().ToString().PadLeft(2, '0')))
            );

            if (dup > 0)
            {
                throw new Exception("Cannot select duplicate time slot");
            }

            operationFixSlot.SupGroup = null;

            unitOfWork.OperationFixSlotRepository.Add(operationFixSlot);

            unitOfWork.Save();

            return true;
        }

        public async Task<bool> Delete(decimal id)
        {
            var operationFixSlot = await Task.Run<OperationFixSlot?>(() => unitOfWork.OperationFixSlotRepository.GetOperationById(id));

            unitOfWork.OperationFixSlotRepository.Delete(operationFixSlot);

            unitOfWork.Save();

            return true;
        }



    }
}
