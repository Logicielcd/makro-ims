using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Hubs;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sieve.Models;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class SupplierController : ApiControllerBase
    {
        private readonly SupplierService _supplierService;        
        private readonly IHubContext<SupplierHub> _hub;

        public SupplierController(
            IHubContext<SupplierHub> hub,
            [FromServices] SupplierService supplierService
            )
        {
            _supplierService = supplierService;
            _hub = hub;
        
        }

        [HttpPost("pages")]
        public async Task<IActionResult> GetPages(SieveModel sieveModel)
        {
            var result = await _supplierService.GetSupplierPaged(sieveModel);
            return OkResponse(result);
        }


        [HttpPost("export")]
        public async Task<IActionResult> GetExport(SieveModel sieveModel)
        {
            var result = await _supplierService.GetSupplierExport(sieveModel);
            return OkResponse(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _supplierService.GetSupplieres(userName);

            
            return OkResponse(result);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetByUser()
        {
            var userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _supplierService.GetSupplieres(userName);

            return OkResponse(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _supplierService.GetById(id);

            return OkResponse(result);
        }

        [HttpGet("bysupgroup/{id}")]
        public async Task<IActionResult> GetBySupGroupId(int id)
        {
            var result = await _supplierService.GetBySupGroupId(id);

            return OkResponse(result);
        }

        [HttpGet("supGroup/{id}")]
        public async Task<IActionResult> GetSupGroup(int id)
        {
            var result = await _supplierService.GetSupGroupById(id);

            return OkResponse(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Add(Supplier supplier)
        {
            var result = await _supplierService.Add(supplier);

            var newSup = await _supplierService.GetNewSupplieres();

            await _hub.Clients.All.SendAsync("supplier", newSup);

            return OkResponse(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] SupplierDto supplier)
        {
            var userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _supplierService.UpdateSupplieres(supplier,userName);

            var newSup = await _supplierService.GetNewSupplieres();

            await _hub.Clients.All.SendAsync("supplier", newSup);


            return OkResponse(result);
        }


        [HttpPost("updatesup")]
        public async Task<IActionResult> UpdateSup([FromBody] Supplier supplier)
        {            
            var result = await _supplierService.Update(supplier);

            var newSup = await _supplierService.GetNewSupplieres();

            await _hub.Clients.All.SendAsync("supplier", newSup);

            return OkResponse(result);
        }

        [HttpGet("newsupplier")]
        public async Task<ActionResult> NewSupplier()
        {
            try
            {
                var result = await _supplierService.GetNewSupplieres();

                await _hub.Clients.All.SendAsync("supplier", result);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] SupplierDto supplier)
        {
            var userName = this.User.Identities.FirstOrDefault().Name;
            var result = await _supplierService.UpdateSupplieres(supplier, userName);

            var newSup = await _supplierService.GetNewSupplieres();

            await _hub.Clients.All.SendAsync("supplier", newSup);


            return OkResponse(result);
        }

    }
}
