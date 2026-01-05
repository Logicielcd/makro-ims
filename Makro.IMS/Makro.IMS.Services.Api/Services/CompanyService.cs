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
    public class CompanyService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;        

        public CompanyService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public async Task<List<Company>> GetCompanies()
        {
            return await Task.Run<List<Company>>(() => unitOfWork.CompanyRepository.GetCompanies().ToList());
        }

        public async Task<Company?> GetById(string companyCode)
        {
            return await Task.Run<Company?>(() => unitOfWork.CompanyRepository.GetCompanyByCompanyCode(companyCode));
        }



    }
}
