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
    public class PoCommentService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public PoCommentService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public PoCommentService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PoComment?> GetByPo(string poNo)
        {
            return await Task.Run<PoComment?>(() => unitOfWork.PoCommentRepository.GetPoCommentByPoNo(poNo));
        }

        public async Task<bool> AddDoor(PoComment data)
        {                        
            await Task.Run(() => unitOfWork.PoCommentRepository.Add(data));
            unitOfWork.Save();            
            return true;
        }

        public async Task<bool> Update(PoComment data)
        {
            // Check duplicate
            var po = unitOfWork.PoCommentRepository.GetPoCommentByPoNo(data.Po);

            if (po == null)
            {
                po = new PoComment();
                po = data;
                po.Comment7 = "";
                po.Comment8 = "";
                po.DateTimeStamp = DateTime.Now;
                await Task.Run(() => unitOfWork.PoCommentRepository.Add(po));
                unitOfWork.Save();
            }
            else
            {
                po.Comment1 = data.Comment1;
                po.Comment2 = data.Comment2;
                po.Comment3 = data.Comment3;
                po.Comment4 = data.Comment4;
                po.Comment5 = data.Comment5;
                po.Comment6 = data.Comment6;
                po.Comment7 = "";
                po.Comment8 = "";                
                po.DateTimeStamp = DateTime.Now;
                await Task.Run(() => unitOfWork.PoCommentRepository.Update(po));
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<bool> Delete(string poNo)
        {
            // Check duplicate
            var po = unitOfWork.PoCommentRepository.GetPoCommentByPoNo(poNo);

            if (po == null)
            {
                throw new Exception("PO not found.");
            }
            else
            {
                await Task.Run(() => unitOfWork.PoCommentRepository.Delete(po));
                unitOfWork.Save();
            }
            return true;
        }

    }
}
