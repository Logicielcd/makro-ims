using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;
using System.Linq;

namespace Makro.IMS.Services.Api.Services
{
    public class WarehouseOperationCapacityService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public WarehouseOperationCapacityService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public WarehouseOperationCapacityService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<WarehouseOperationCapacity>> GetWarehouseCapacities()
        {
            return await Task.Run<List<WarehouseOperationCapacity>>(() => unitOfWork.WarehouseOperationCapacityRepository.GetWarehousesCapacity().ToList());
        }

        public async Task<PagedResult<WarehouseOperationCapacity>> GetWarehouseCapacitiesPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.WarehouseOperationCapacityRepository.GetWarehousesCapacityPaged();
            return await _sieveProcessor.GetPagedAsync<WarehouseOperationCapacity>(queryable, sieveModel);
        }
        public async Task<WarehouseOperationCapacity?> GetById(int id)
        {
            return await Task.Run<WarehouseOperationCapacity?>(() => unitOfWork.WarehouseOperationCapacityRepository.GetWarehouseCapacirtyById(id));
        }

        public async Task<bool> CreateWarehouseOperationCapacity(WarehouseOperationCapacity warehouseCapacity)
        {
            
            if (warehouseCapacity == null)
            {
                return false;
            }
            else
            {
                unitOfWork.WarehouseOperationCapacityRepository.Add(warehouseCapacity);
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> UpdateWarehouseOperationCapacity(WarehouseOperationCapacity warehouseCapacity)
        {            
            if (warehouseCapacity == null)
            {
                return false;
            }
            else
            {
                unitOfWork.WarehouseOperationCapacityRepository.Update(warehouseCapacity);
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> Delete(int warehouseCapacityId)
        {

            var warehouseCapacity = unitOfWork.WarehouseOperationCapacityRepository.GetWarehouseCapacirtyById(warehouseCapacityId);
            if (warehouseCapacity == null)
            {
                return false;
            }
            else
            {
                unitOfWork.WarehouseOperationCapacityRepository.Remove(warehouseCapacity);
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<List<WarehouseOperationCapacity>?> GetByWarehouse(string warehouse)
        {
            return await Task.Run<List<WarehouseOperationCapacity>?>(() => unitOfWork.WarehouseOperationCapacityRepository.GetByWarehouse(warehouse).ToList());
        }

        public async Task<List<WarehouseOperationCapacityDto>?> GetCapacityByBookingDate(DateTime bookingDate, string warehouse)
        {
            var result = await Task.Run<List<BookingCapacity>?>(() => unitOfWork.PoListRepository.GetCapacity(bookingDate, warehouse).ToList());
            var whseCaps = new List<WarehouseOperationCapacityDto>();
            WarehouseOperationCapacityDto whseCap = new WarehouseOperationCapacityDto();

            var whse = await Task.Run<Warehouse>(() => unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouse));
            var whseCapacity = await Task.Run<WarehouseOperationCapacity>(() => unitOfWork.WarehouseOperationCapacityRepository.GetByWarehouseBookingDate(warehouse,bookingDate));

            for (int i = 0; i < result.Count(); i++)
            {
                whseCap = new WarehouseOperationCapacityDto();
                whseCap.WarehouseCode = warehouse;                
                whseCap.BookingDateTime = Convert.ToDateTime(result[i].T + ":00:00.000");
                whseCap.CON = result[i].CON;
                whseCap.NON = result[i].NON;
                whseCap.FULL_PL = result[i].FULL_PL;
                whseCap.CUBE_FULL = result[i].CUBE_FULL;
                whseCap.CUBE_NON = result[i].CUBE_NON;
                whseCap.CUBE_CON = result[i].CUBE_CON;

                if(whseCapacity != null && !string.IsNullOrEmpty(whseCapacity.WarehouseCode))
                {
                    whseCap.MAX_CON = whseCapacity.MaxConPerHour;
                    whseCap.MAX_NON = whseCapacity.MaxNonPerHour;
                    whseCap.MAX_FULL_PL = whseCapacity.MaxFullPerHour;
                    whseCap.MAX_CUBE_FULL = whseCapacity.MaxCFullPerHour;
                    whseCap.MAX_CUBE_NON = whseCapacity.MaxCNonPerHour;
                    whseCap.MAX_CUBE_CON = whseCapacity.MaxCConPerHour;
                }
                else
                {
                    whseCap.MAX_CON = whse.MaxConPerHour;
                    whseCap.MAX_NON = whse.MaxNonPerHour;
                    whseCap.MAX_FULL_PL = whse.MaxFullPerHour;
                    whseCap.MAX_CUBE_FULL = whse.MaxCFullPerHour;
                    whseCap.MAX_CUBE_NON = whse.MaxCNonPerHour;
                    whseCap.MAX_CUBE_CON = whse.MaxCConPerHour;
                }

                whseCaps.Add(whseCap);
            }


            return whseCaps;

        }


    }
}
