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
    public class QueueSequenceService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public QueueSequenceService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public QueueSequenceService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<QueueSequence> GetQueueSequence(string warehouseCode,string operationType)
        {
            int lastQueue = 0;

            QueueSequence queue = await Task.Run<QueueSequence>(() => unitOfWork.QueueSequenceRepository.GetQueueSeq(warehouseCode, operationType));

            if (queue == null)
            {
                // create new queue sequence
                QueueSequence seq = new QueueSequence();
                seq.WarehouseCode = warehouseCode;
                seq.OperationType = operationType;
                seq.LastSequence = 1;
                seq.QueueDate = DateTime.Now.Date;

                unitOfWork.QueueSequenceRepository.Add(seq);

                queue = seq;
            }
            else
            {               
                if (queue.QueueDate.Value == DateTime.Now.Date)
                {                 
                    queue.LastSequence = queue.LastSequence + 1;

                    queue.LastSequence = validateQueue(warehouseCode, operationType, queue.LastSequence);

                }
                else
                {             
                    queue.LastSequence = 1;
                    queue.QueueDate = DateTime.Now.Date;
                }

                unitOfWork.QueueSequenceRepository.Update(queue);
            }

            unitOfWork.Save();

            return queue;
        }

        private decimal validateQueue(string warehouseCode,string operationType, decimal lastSeq)
        {
            var bookingTrucks = unitOfWork.BookingTruckCheckInRepository.GetLastQueueId(DateTime.Now.Date);
            var nextSeq = lastSeq;

            foreach (var truck in bookingTrucks)
            {
                var hdr = unitOfWork.BookingHeaderRepository.GetBookingHeaderById(Convert.ToInt32(truck.InternalHeaderKey));

                if (hdr.WarehouseCode == warehouseCode && hdr.MerchType == operationType)
                {
                    if (truck.QueueSeq == lastSeq)
                    {
                        nextSeq += 1;
                        nextSeq = validateQueue(warehouseCode, operationType, nextSeq);
                    }
                    else
                    {
                        //nextSeq = lastSeq;
                        //return nextSeq;
                    }
                }
            }

            return nextSeq;
        }

    }
}
