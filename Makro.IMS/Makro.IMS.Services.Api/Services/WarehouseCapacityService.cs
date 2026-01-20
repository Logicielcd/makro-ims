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
    public class WarehouseCapacityService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public WarehouseCapacityService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public WarehouseCapacityService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<WarehouseCapacity>> GetWarehouseCapacities()
        {
            return await Task.Run<List<WarehouseCapacity>>(() => unitOfWork.WarehouseCapacityRepository.GetWarehousesCapacity().ToList());
        }

        public async Task<PagedResult<WarehouseCapacity>> GetWarehouseCapacitiesPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.WarehouseCapacityRepository.GetWarehousesCapacityPaged();
            return await _sieveProcessor.GetPagedAsync<WarehouseCapacity>(queryable, sieveModel);
        }
        public async Task<WarehouseCapacity?> GetById(int id)
        {
            return await Task.Run<WarehouseCapacity?>(() => unitOfWork.WarehouseCapacityRepository.GetWarehouseCapacirtyById(id));
        }

        public async Task<bool> CreateWarehouseCapacity(WarehouseCapacity warehouseCapacity)
        {
            
            if (warehouseCapacity == null)
            {
                return false;
            }
            else
            {
                unitOfWork.WarehouseCapacityRepository.Add(warehouseCapacity);
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> UpdateWarehouseCapacity(WarehouseCapacity warehouseCapacity)
        {            
            if (warehouseCapacity == null)
            {
                return false;
            }
            else
            {
                unitOfWork.WarehouseCapacityRepository.Update(warehouseCapacity);
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<bool> Delete(int warehouseCapacityId)
        {

            var warehouseCapacity = unitOfWork.WarehouseCapacityRepository.GetWarehouseCapacirtyById(warehouseCapacityId);
            if (warehouseCapacity == null)
            {
                return false;
            }
            else
            {
                unitOfWork.WarehouseCapacityRepository.Remove(warehouseCapacity);
                unitOfWork.Save();
            }

            return true;
        }

        public async Task<List<WarehouseCapacity>?> GetByWarehouse(string warehouse)
        {
            return await Task.Run<List<WarehouseCapacity>?>(() => unitOfWork.WarehouseCapacityRepository.GetByWarehouse(warehouse).ToList());
        }

        public async Task<List<WarehouseCapacityDto>?> GetCapacityByBookingDate(DateTime bookingDate, string warehouse)
        {
            var result = await Task.Run<List<BookingCapacity>?>(() => unitOfWork.PoListRepository.GetCapacity(bookingDate, warehouse).ToList());
            var whseCaps = new List<WarehouseCapacityDto>();
            WarehouseCapacityDto whseCap = new WarehouseCapacityDto();

            var whse = await Task.Run<Warehouse>(() => unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouse));
            var whseCapacity = await Task.Run<WarehouseCapacity>(() => unitOfWork.WarehouseCapacityRepository.GetByWarehouseBookingDate(warehouse,bookingDate));

            DateTime startBooking = bookingDate.Date;
            DateTime endBooking = bookingDate.Date;

            if(whse.FirstTimeOfDay > whse.EndTimeOfDay)
            {
                startBooking = startBooking.AddDays(-1);
            }

            int h = (whse.FirstTimeOfDay.Value * 60) / 60;
            int m = (whse.FirstTimeOfDay.Value * 60) % 60;

            startBooking = new DateTime(startBooking.Year,startBooking.Month,startBooking.Day,h,m,0);

            h = (whse.EndTimeOfDay.Value * 60) / 60;
            m = (whse.EndTimeOfDay.Value * 60) % 60;

            if(h == 24)
            {
                h = 23;
                m = 59;
            }

            endBooking = new DateTime(endBooking.Year, endBooking.Month, endBooking.Day, h, m, 0);

            for (int i = 0; i < result.Count(); i++)
            {
                whseCap = new WarehouseCapacityDto();
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

            whseCaps = whseCaps.Where(x => x.BookingDateTime >= startBooking && x.BookingDateTime <= endBooking).ToList();
            return whseCaps;

        }

        public async Task<List<WarehouseCapacityDto>?> GetOperationCapacityByBookingDate(DateTime bookingDate, string warehouse,string operation)
        {
            var result = await Task.Run<List<BookingCapacity>?>(() => unitOfWork.PoListRepository.GetCapacityOperation(bookingDate, warehouse,operation).ToList());
            var whseCaps = new List<WarehouseCapacityDto>();
            WarehouseCapacityDto whseCap = new WarehouseCapacityDto();

            var whse = await Task.Run<Warehouse>(() => unitOfWork.WarehouseRepository.GetWarehouseByWhseCode(warehouse));
            var whseCapacity = await Task.Run<WarehouseCapacity>(() => unitOfWork.WarehouseCapacityRepository.GetByWarehouseBookingDate(warehouse, bookingDate));

            DateTime startBooking = bookingDate.Date;
            DateTime endBooking = bookingDate.Date;

            if (whse.FirstTimeOfDay > whse.EndTimeOfDay)
            {
                startBooking = startBooking.AddDays(-1);
            }

            int h = (whse.FirstTimeOfDay.Value * 60) / 60;
            int m = (whse.FirstTimeOfDay.Value * 60) % 60;

            startBooking = new DateTime(startBooking.Year, startBooking.Month, startBooking.Day, h, m, 0);

            h = (whse.EndTimeOfDay.Value * 60) / 60;
            m = (whse.EndTimeOfDay.Value * 60) % 60;

            if (h == 24)
            {
                h = 23;
                m = 59;
            }

            endBooking = new DateTime(endBooking.Year, endBooking.Month, endBooking.Day, h, m, 0);

            for (int i = 0; i < result.Count(); i++)
            {
                whseCap = new WarehouseCapacityDto();
                whseCap.WarehouseCode = warehouse;
                whseCap.BookingDateTime = Convert.ToDateTime(result[i].T + ":00.000");
                whseCap.CON = result[i].CON;
                whseCap.NON = result[i].NON;
                whseCap.FULL_PL = result[i].FULL_PL;
                whseCap.CUBE_FULL = result[i].CUBE_FULL;
                whseCap.CUBE_NON = result[i].CUBE_NON;
                whseCap.CUBE_CON = result[i].CUBE_CON;

                /*
                if (whseCapacity != null && !string.IsNullOrEmpty(whseCapacity.WarehouseCode))
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
                */

                whseCaps.Add(whseCap);
            }

            whseCaps = whseCaps.Where(x => x.BookingDateTime >= startBooking && x.BookingDateTime <= endBooking).ToList();
            return whseCaps;

        }

    }
}
