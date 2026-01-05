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
    public class DoorService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public DoorService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public DoorService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<Door>> GetDoors()
        {
            return await Task.Run<List<Door>>(() => unitOfWork.DoorRepository.GetDoors().ToList());
        }

        public async Task<PagedResult<Door>> GetDoorPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.DoorRepository.GetDoorPaged();
            return await _sieveProcessor.GetPagedAsync<Door>(queryable, sieveModel);
        }

        public async Task<Door?> GetById(int doorId)
        {
            return await Task.Run<Door?>(() => unitOfWork.DoorRepository.GetDoorById(doorId));
        }

        public async Task<bool> AddDoor(Door door)
        {
            // Check duplicate
            var cntDoor = unitOfWork.DoorRepository.GetDoors().ToList().Count(x=>x.DoorName == door.DoorName
            && x.WarehouseCode == door.WarehouseCode);

            if (cntDoor > 0)
            {
                throw new Exception("Door already existing.");             
            }
            else
            {
                var doorId = unitOfWork.DoorRepository.GetDoorKey();
                unitOfWork.Save();
                
                door.InternalDoorId = doorId;
                await Task.Run(() => unitOfWork.DoorRepository.Add(door));
                
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
