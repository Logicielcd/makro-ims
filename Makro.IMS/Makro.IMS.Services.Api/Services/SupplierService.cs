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
    public class SupplierService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public SupplierService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public SupplierService(
           ISieveProcessor sieveProcessor
           )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PagedResult<Supplier>> GetSupplierPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.SupplierRepository.GetSupplierPaged();
            return await _sieveProcessor.GetPagedAsync<Supplier>(queryable, sieveModel);
        }

        public async Task<PagedResult<Supplier>> GetSupplierExport(SieveModel sieveModel)
        {
            var queryable = unitOfWork.SupplierRepository.GetSupplierPaged();
            return await _sieveProcessor.GetPagedExportAsync<Supplier>(queryable, sieveModel);
        }

        public async Task<List<SupplierDto>> GetSupplieres()
        {
            List<Supplier> suppliers = await Task.Run<List<Supplier>>(() => unitOfWork.SupplierRepository.GetSuppliers().ToList());

            List<SupplierDto> supplierDtos = new List<SupplierDto>();

            List<SupplierGroup> supplierGroups = await Task.Run<List<SupplierGroup>>(() => unitOfWork.SupplierGroupRepository.GetAll().ToList());

            for (int i = 0; i < suppliers.Count; i++)
            {
                SupplierDto supplierDto = new SupplierDto();
                SupplierGroup supplierGroup = supplierGroups.FirstOrDefault(x => x.InternalSupGroupId == suppliers[i].InternalGroupId);

                supplierDto.InternalSupId = suppliers[i].InternalSupId;
                supplierDto.InternalSupGroupId = suppliers[i].InternalSupGroupId;
                supplierDto.InternalGroupId = suppliers[i].InternalGroupId;
                supplierDto.SupCode = suppliers[i].SupCode;
                supplierDto.SupName = suppliers[i].SupName;

                if (supplierGroup != null)
                {
                    supplierDto.Remark = supplierGroup.Remark;
                    supplierDto.ContactName = supplierGroup.ContactName;
                    supplierDto.PhoneNumber = supplierGroup.PhoneNumber;
                    supplierDto.ContactEMail = supplierGroup.ContactEMail;
                }

                supplierDtos.Add(supplierDto);
            }


            
            return supplierDtos;

        }

        public async Task<List<SupplierDto>> GetSupplieres(string userName)
        {
            List<Supplier> suppliers = await Task.Run<List<Supplier>>(() => unitOfWork.SupplierRepository.GetSuppliers().ToList());

            UserMaster userMaster = unitOfWork.UserMasterRepository.GetUserMasterByUserId(userName);
            List<UserSupplierGroup> userSupplierGroups = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(userName).ToList();

            List<SupplierDto> supplierDtos = new List<SupplierDto>();

            if (userMaster.InternalSupGroupId != null)
            {
                if (userSupplierGroups.Count > 0)
                {
                    suppliers = suppliers.Where(x => x.InternalGroupId == userMaster.InternalSupGroupId || userSupplierGroups.Any(y => y.InternalSupGroup == x.InternalGroupId)).ToList();
                    
                }
                else
                {
                    suppliers = suppliers.Where(x => x.InternalGroupId == userMaster.InternalSupGroupId).ToList();
                }

            }


            List<SupplierGroup> supplierGroups = await Task.Run<List<SupplierGroup>>(() => unitOfWork.SupplierGroupRepository.GetAll().ToList());

            for (int i = 0; i < suppliers.Count; i++)
            {
                SupplierDto supplierDto = new SupplierDto();
                SupplierGroup supplierGroup = supplierGroups.FirstOrDefault(x => x.InternalSupGroupId == suppliers[i].InternalGroupId);

                supplierDto.InternalSupId = suppliers[i].InternalSupId;
                supplierDto.InternalSupGroupId = suppliers[i].InternalSupGroupId;
                supplierDto.InternalGroupId = suppliers[i].InternalGroupId;
                supplierDto.SupCode = suppliers[i].SupCode;
                supplierDto.SupName = suppliers[i].SupName;

                if (supplierGroup != null)
                {
                    supplierDto.Remark = supplierGroup.Remark;
                    supplierDto.ContactName = supplierGroup.ContactName;
                    supplierDto.PhoneNumber = supplierGroup.PhoneNumber;
                    supplierDto.ContactEMail = supplierGroup.ContactEMail;
                }

                supplierDtos.Add(supplierDto);
            }

            return supplierDtos;

        }

        public async Task<Supplier?> GetById(string supplierCode)
        {
            return await Task.Run<Supplier?>(() => unitOfWork.SupplierRepository.GetSupplierBySupCode(supplierCode));
        }

        public async Task<List<SupplierDto>> GetBySupGroupId(int supGroupId)
        {

            List<Supplier> suppliers = await Task.Run<List<Supplier>>(() => unitOfWork.SupplierRepository.GetSupplierBySupGroupId(supGroupId).ToList());

            List<SupplierDto> supplierDtos = new List<SupplierDto>();

            List<SupplierGroup> supplierGroups = await Task.Run<List<SupplierGroup>>(() => unitOfWork.SupplierGroupRepository.GetAll().ToList());

            for (int i = 0; i < suppliers.Count; i++)
            {
                SupplierDto supplierDto = new SupplierDto();
                SupplierGroup supplierGroup = supplierGroups.FirstOrDefault(x => x.InternalSupGroupId == suppliers[i].InternalGroupId);

                supplierDto.InternalSupId = suppliers[i].InternalSupId;
                supplierDto.InternalSupGroupId = suppliers[i].InternalSupGroupId;
                supplierDto.InternalGroupId = suppliers[i].InternalGroupId;
                supplierDto.SupCode = suppliers[i].SupCode;
                supplierDto.SupName = suppliers[i].SupName;

                if (supplierGroup != null)
                {
                    supplierDto.Remark = supplierGroup.Remark;
                    supplierDto.ContactName = supplierGroup.ContactName;
                    supplierDto.PhoneNumber = supplierGroup.PhoneNumber;
                    supplierDto.ContactEMail = supplierGroup.ContactEMail;
                    supplierDto.MobileNumber = supplierGroup.MobileNumber;
                }

                supplierDtos.Add(supplierDto);
            }


            return supplierDtos;
        }

        public async Task<SupplierGroup> GetSupGroupById(int supGroupId)
        {
            return await Task.Run<SupplierGroup>(() => unitOfWork.SupplierGroupRepository.GetSupplierGroupById(supGroupId));
        }

        public async Task<bool> Add(Supplier supplier)
        {
            // Check duplicate
            var sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(supplier.SupCode);

            if (sup != null)
            {
                throw new Exception("Supplier already existing.");
            }
            else
            {
                var supId = unitOfWork.SupplierRepository.GetKey();
                unitOfWork.Save();

                supplier.InternalSupId = supId;
                supplier.CompanyCode = "88";
                supplier.CreateDate = DateTime.Now;

                await Task.Run(() => unitOfWork.SupplierRepository.Add(supplier));

                unitOfWork.Save();
            }
            return true;
        }

        public async Task<Supplier> Update(Supplier supplier)
        {

            Supplier sup = await Task.Run<Supplier>(() => unitOfWork.SupplierRepository.GetSupplierBySupCode(supplier.SupCode));

            if (sup == null)
            {
                throw new Exception("Not found supplier.Please contact admin \r\n ทำการติดต่อผู้ดูแลระบบเนื่องจากไม่มีการกำหนด supplier ในระบบ");
            }
            else
            {
                sup.ModDate = DateTime.Now;
                sup.UserStamp = supplier.UserStamp;
                sup.SupName = supplier.SupName;
                sup.SupCode = supplier.SupCode;
                sup.InternalGroupId = supplier.InternalGroupId;

                unitOfWork.SupplierRepository.Update(sup);

                unitOfWork.Save();
            }

            return supplier;

        }

        public async Task<SupplierDto> UpdateSupplieres(SupplierDto supplier,string userName)
        {
            
            Supplier sup = await Task.Run<Supplier>(() => unitOfWork.SupplierRepository.GetSupplierBySupCode(supplier.SupCode));

            if (sup.InternalSupId == null)
            {
                throw new Exception("Not found supplier group.Please contact admin \r\n ทำการติดต่อผู้ดูแลระบบเนื่องจากไม่มีการกำหนด supplier group ในระบบ");
            }
            else
            {
                SupplierGroup supplierGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(sup.InternalGroupId.Value);

                supplierGroup.ContactName = supplier.ContactName;
                supplierGroup.PhoneNumber = supplier.PhoneNumber;
                supplierGroup.MobileNumber = supplier.MobileNumber;
                supplierGroup.ContactEMail = supplier.ContactEMail;
                supplierGroup.UserStamp = userName;
                supplierGroup.ModDate = DateTime.Now;

                unitOfWork.SupplierGroupRepository.UpdateSupplierContact(supplierGroup);

                unitOfWork.Save();
            }
            
            return supplier;

        }

        public async Task<List<SupplierDto>> GetNewSupplieres()
        {
            SupplierGroup supplierGroups = await Task.Run<SupplierGroup>(() => unitOfWork.SupplierGroupRepository.GetAll().FirstOrDefault(x=>x.SupName == "NEW SUP"));

            List<Supplier> suppliers = await Task.Run<List<Supplier>>(() => unitOfWork.SupplierRepository.GetSuppliers().ToList());

            List<SupplierDto> supplierDtos = new List<SupplierDto>();

            suppliers = suppliers.Where(x=>x.InternalGroupId == supplierGroups.InternalSupGroupId).ToList();

            
            for (int i = 0; i < suppliers.Count; i++)
            {
                SupplierDto supplierDto = new SupplierDto();
                
                supplierDto.InternalSupId = suppliers[i].InternalSupId;
                supplierDto.InternalSupGroupId = suppliers[i].InternalSupGroupId;
                supplierDto.InternalGroupId = suppliers[i].InternalGroupId;
                supplierDto.SupCode = suppliers[i].SupCode;
                supplierDto.SupName = suppliers[i].SupName;

                if (supplierGroups != null)
                {
                    supplierDto.Remark = supplierGroups.Remark;
                    supplierDto.ContactName = supplierGroups.ContactName;
                    supplierDto.PhoneNumber = supplierGroups.PhoneNumber;
                    supplierDto.ContactEMail = supplierGroups.ContactEMail;
                }

                supplierDtos.Add(supplierDto);
            }


            return supplierDtos;

        }

        public async Task<SupplierDto> DeleteSupplieres(SupplierDto supplier, string userName)
        {

            Supplier sup = await Task.Run<Supplier>(() => unitOfWork.SupplierRepository.GetSupplierBySupCode(supplier.SupCode));

            if (sup.InternalSupId == null)
            {
                throw new Exception("Not found supplier group.Please contact admin \r\n ทำการติดต่อผู้ดูแลระบบเนื่องจากไม่มีการกำหนด supplier group ในระบบ");
            }
            else
            {
                unitOfWork.SupplierRepository.Delete(sup);
                unitOfWork.Save();
            }

            return supplier;

        }

    }
}
