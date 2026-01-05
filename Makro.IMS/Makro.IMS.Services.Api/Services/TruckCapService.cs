using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
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
    public class TruckCapService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public TruckCapService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public TruckCapService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<TruckCap>> GetTruckCaps()
        {
            return await Task.Run<List<TruckCap>>(() => unitOfWork.TruckCapRepository.GetTruckCaps().ToList());
        }

        //public async Task<PagedResult<TruckCap>> GetTruckCapPaged(SieveModel sieveModel)
        //{
        //    var queryable = unitOfWork.DoorRepository.GetDoorPaged();
        //    return await _sieveProcessor.GetPagedAsync<Door>(queryable, sieveModel);
        //}

        public async Task<List<TruckCap>> GetTruckCapsByWhseOperation(string warehouseCode,string operationType)
        {
            return await Task.Run<List<TruckCap>>(() => unitOfWork.TruckCapRepository.GetTruckCaps().ToList().Where(x=>x.WarehouseCode == warehouseCode && x.OperationType == operationType).ToList());
        }

        public async Task<bool> Add(TruckCap truck)
        {
            // Check duplicate
            var cntTruck = unitOfWork.TruckCapRepository.GetTruckCaps().ToList().Count(x=>x.OperationType == truck.OperationType
            && x.WarehouseCode == truck.WarehouseCode && x.InternalTruckId == truck.InternalTruckId);

            if (cntTruck > 0)
            {
                throw new Exception("Truck Cap already existing.");             
            }
            else
            {                                
                await Task.Run(() => unitOfWork.TruckCapRepository.Add(truck));
                
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Update(Door door)
        {
            // Check duplicate
            var doorUpdate = unitOfWork.DoorRepository.GetDoors().ToList().FirstOrDefault(x => x.DoorName == door.DoorName
            && x.WarehouseCode == door.WarehouseCode);

            if (doorUpdate == null)
            {
                throw new Exception("Door not found.");
            }
            else
            {
                doorUpdate.DoorArea = door.DoorArea;
                doorUpdate.Sequence = door.Sequence;
                doorUpdate.Active = true;
                doorUpdate.LoadingType = door.LoadingType;
                doorUpdate.TruckType = door.TruckType;

                await Task.Run(() => unitOfWork.DoorRepository.Update(doorUpdate));
                unitOfWork.Save();                
            }
            return true;
        }

        public async Task<bool> Delete(int doorId)
        {
            // Check duplicate
            var door = unitOfWork.DoorRepository.GetDoorById(doorId);

                        if (door == null)
            {
                throw new Exception("Door not found.");
            }
            else
            {
                unitOfWork.DoorRepository.Delete(door);
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<List<Door>> GetDoorsByWarehouse(string warehouseCode)
        {
         
            return await Task.Run<List<Door>>(() => unitOfWork.DoorRepository.GetDoorsByWarehouse(warehouseCode).ToList());
        }

        public async Task<List<Door>> GetDoorsBySupCode(string warehouseCode,string supCode)
        {
            var sup = unitOfWork.SupplierRepository.GetSupplierBySupCode(supCode);
            var supGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(sup.InternalGroupId.Value);
           
            string doorType = supGroup == null ? "" : supGroup.Remark!;

            return await Task.Run<List<Door>>(() => unitOfWork.DoorRepository.GetDoorsByDoorType(warehouseCode, doorType).ToList());
        }

        public async Task<List<Door>> GetDoorsByOperationType(string warehouseCode, string operationType,int internalTruckCheckinId)
        {
            var truckCheckIn = unitOfWork.BookingTruckCheckInRepository.GetBookingCheckInById(internalTruckCheckinId);
            var truck = unitOfWork.TruckMasterRepository.GetTrucks().FirstOrDefault(x => x.InternalTruckId == truckCheckIn.InternalTruckId);

            return await Task.Run<List<Door>>(() => unitOfWork.DoorRepository.GetDoorsAvailableByOperationType(warehouseCode, operationType,truck.TruckCode).ToList());
        }




    }
}
