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
    public class WarehouseService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public WarehouseService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public WarehouseService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<Warehouse>> GetWarehouses()
        {
            return await Task.Run<List<Warehouse>>(() => unitOfWork.WarehouseRepository.GetWarehouses().ToList());
        }
        public async Task<PagedResult<Warehouse>> GetWarehousePaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.WarehouseRepository.GetWarehousesPaged();
            return await _sieveProcessor.GetPagedAsync<Warehouse>(queryable, sieveModel);
        }

        public async Task<Warehouse?> GetById(string warehouseCode)
        {
            return await Task.Run<Warehouse?>(() => unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouseCode));
        }

        public async Task<bool> Add(Warehouse warehouse)
        {
            Warehouse whse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouse.WarehouseCode);

            if (whse == null)
            {
                await Task.Run(() => unitOfWork.WarehouseRepository.Add(warehouse));
                unitOfWork.Save();
            }
            else
            {
                throw new Exception("Warehouse already existing");
            }

            
            return true;
        }

        public async Task<bool> Update(Warehouse warehouse)
        {
            Warehouse whse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouse.WarehouseCode);

            if (whse == null)
            {
                throw new Exception("Warehouse not found");
            }
            else
            {
                whse.WarehouseMain = warehouse.WarehouseMain;
                whse.WarehouseLevel = warehouse.WarehouseLevel;
                whse.WarehouseName = warehouse.WarehouseName;
                whse.CompanyCode = warehouse.CompanyCode;
                whse.Active = warehouse.Active;
                whse.Address1 = warehouse.Address1;
                whse.Address2 = warehouse.Address2;
                whse.Address3 = warehouse.Address3;
                whse.City = warehouse.City;
                whse.Country = warehouse.Country;
                whse.Zipcode = warehouse.Zipcode;

                whse.ContactName = warehouse.ContactName;                
                whse.ContactEMail = warehouse.ContactEMail;
                whse.PhoneNumber = warehouse.PhoneNumber;
                whse.MobileNumber = warehouse.MobileNumber;

                whse.MaxCAllPerHour = warehouse.MaxCAllPerHour;
                whse.MaxCConPerHour = warehouse.MaxCConPerHour;
                whse.MaxCNonPerHour = warehouse.MaxCNonPerHour;
                whse.MaxCFullPerHour = warehouse.MaxCFullPerHour;
                whse.MaxAllPerHour = warehouse.MaxAllPerHour;
                whse.MaxConPerHour = warehouse.MaxConPerHour;
                whse.MaxNonPerHour = warehouse.MaxNonPerHour;
                whse.MaxFullPerHour = warehouse.MaxFullPerHour;
                                
                whse.BookingIdPrefix = warehouse.BookingIdPrefix;
                whse.BookingIdRunning = warehouse.BookingIdRunning;

                whse.OnlineBookingIdPrefix = warehouse.OnlineBookingIdPrefix;
                whse.OnlineBookingIdRunning = warehouse.OnlineBookingIdRunning;
                whse.FixDoor = warehouse.FixDoor;

                whse.FirstTimeOfDay = warehouse.FirstTimeOfDay;
                whse.EndTimeOfDay = warehouse.EndTimeOfDay;
                whse.TimeWidth = warehouse.TimeWidth;
                whse.TimeIncreaseStep = warehouse.TimeIncreaseStep;
                whse.Note = warehouse.Note;
               
                whse.ModDate = DateTime.Now;
                whse.UserStamp = warehouse.UserStamp;
                
                whse.PoAfterPeriod = warehouse.PoAfterPeriod;
                whse.PoBeforePeriod = warehouse.PoBeforePeriod;
                whse.AdvanceBookingPeriod = warehouse.AdvanceBookingPeriod;
                whse.AdvanceCheckinTime = warehouse.AdvanceCheckinTime;
                whse.AdvanceBookingDay  = warehouse.AdvanceBookingDay;
                whse.LateCheckinTime = warehouse.LateCheckinTime;

                whse.WarehouseWms = warehouse.WarehouseWms;

                whse.CapUom = warehouse.CapUom;

                await Task.Run(() => unitOfWork.WarehouseRepository.UpdateWarehouse(whse));

                unitOfWork.Save();
            }


            return true;
        }

        public async Task<bool> Delete(string warehouseCode)            
        {
            Warehouse warehouse = unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouseCode);

            if (warehouse != null)
            {
                await Task.Run(() => unitOfWork.WarehouseRepository.Delete(warehouse));
                unitOfWork.Save();
            }
            else
            {
                throw new Exception("Warehouse not found");
            }
            return true;
        }


    }
}
