using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Makro.IMS.Services.Api.Controllers
{
    
    public abstract class ApiControllerBase : ControllerBase
    {
        
        protected ApiControllerBase()
        {
            
        }
        
        protected ActionResult OkResponse(object result = null)
        {
            return Ok(result);
        }

        protected ActionResult BadResponse(object result = null)
        {
            string[] errorMsg = new string[1];
            errorMsg[0] = result.ToString();
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Messages", errorMsg }
            }));
        }

    }

}
