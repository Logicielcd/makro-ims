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
    public class SupplierGroupService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public SupplierGroupService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public SupplierGroupService(
           ISieveProcessor sieveProcessor
           )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PagedResult<SupplierGroup>> GetSupplierGroupPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.SupplierGroupRepository.GetSupplierGroupPaged();
            return await _sieveProcessor.GetPagedAsync<SupplierGroup>(queryable, sieveModel);
        }

        public async Task<List<SupplierGroup>> GetSupplierGroup()
        {
            List <SupplierGroup> supplierGroups = await Task.Run<List<SupplierGroup>>(() => unitOfWork.SupplierGroupRepository.GetAll().ToList() );

            return supplierGroups;
        }

        public async Task<SupplierGroup?> GetById(int supGroupId)
        {
            return await Task.Run<SupplierGroup?>(() => unitOfWork.SupplierGroupRepository.GetSupplierGroupById(supGroupId));
        }

        public async Task<bool> Add(SupplierGroup supGroup)
        {
            // Check duplicate
            var cntSupGroup = unitOfWork.SupplierGroupRepository.GetAll().ToList().Count(x => x.SupName == supGroup.SupName);

            if (cntSupGroup > 0)
            {
                throw new Exception("Supplier Group already existing.");
            }
            else
            {
                var supGroupId = unitOfWork.SupplierGroupRepository.GetKey();
                unitOfWork.Save();

                supGroup.InternalSupGroupId = supGroupId;
                supGroup.CompanyCode = "88";
                supGroup.InternalSupTypeId = 1;
                supGroup.CreateDate = DateTime.Now;
                
                if(supGroup.IsUpCreateBook == null)
                    supGroup.IsUpCreateBook = "N";

                if(supGroup.IsUpPreCheckin == null)
                    supGroup.IsUpPreCheckin = "N";

                if (supGroup.IsVip == null)
                {
                    supGroup.IsVip = "N";
                    supGroup.Warehouses = "";
                }                
                                
                await Task.Run(() => unitOfWork.SupplierGroupRepository.Add(supGroup));

                // create est time
                unitOfWork.EstTimeRepository.CreateEstimateTime(supGroupId);
                

                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Update(SupplierGroup supGroup)
        {
            // Check duplicate
            var supplierGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(supGroup.InternalSupGroupId);

            if (supplierGroup == null)
            {
                throw new Exception("Supplier Group not found.");
            }
            else
            {
                supplierGroup.SupName = supGroup.SupName;
                supplierGroup.ContactEMail = supGroup.ContactEMail;
                supplierGroup.ContactName = supGroup.ContactName;
                supplierGroup.MobileNumber = supGroup.MobileNumber;
                supplierGroup.MaxBookingPreHour = supGroup.MaxBookingPreHour;
                supplierGroup.ComfirmGatePass = supGroup.ComfirmGatePass;
                supplierGroup.IsVip = supGroup.IsVip;
                supplierGroup.IsUpCreateBook = supGroup.IsUpCreateBook;
                supplierGroup.IsUpPreCheckin = supGroup.IsUpPreCheckin;
                supplierGroup.PhoneNumber = supGroup.PhoneNumber;
                supplierGroup.Remark = supGroup.Remark;
                supplierGroup.UserStamp = supGroup.UserStamp;
                supplierGroup.ModDate = DateTime.Now;
                supplierGroup.Warehouses = supGroup.Warehouses;
                supplierGroup.BuyerCode = supGroup.BuyerCode;
                supplierGroup.RmsCode = supGroup.RmsCode;
                supplierGroup.WarehouseCutoff = supGroup.WarehouseCutoff;

                await Task.Run(() => unitOfWork.SupplierGroupRepository.Update(supplierGroup));
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Remove(SupplierGroup supGroup)
        {
            // Check duplicate
            var supplierGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(supGroup.InternalSupGroupId);

            if (supplierGroup == null)
            {
                throw new Exception("Supplier Group not found.");
            }
            else
            {
                await Task.Run(() => unitOfWork.SupplierGroupRepository.Remove(supplierGroup));
                unitOfWork.Save();
            }
            return true;
        }
    }
}
