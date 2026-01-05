using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Helper;
using Makro.IMS.Services.Api.Hubs;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
   // [Authorize]
    [ApiController]
    
    public class UserRegisterController : ApiControllerBase
    {
        private readonly UserMasterService _userMasterService;
        private readonly SupplierGroupService _supplierGroupService;
        private readonly IHubContext<UserRegisterHub> _hub;


        public UserRegisterController(
            IHubContext<UserRegisterHub> hub,
            [FromServices] UserMasterService userMasterService,
            [FromServices] SupplierGroupService supplierGroupService
            )
        {
            _userMasterService = userMasterService;
            _supplierGroupService = supplierGroupService;
            _hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userList = await _userMasterService.GetUsers();

            var supGroups = await _supplierGroupService.GetSupplierGroup();

            List<UserRegisterPendingDto> userPendings = new List<UserRegisterPendingDto>();

            foreach (var user in userList.Where(x=>x.Approved != null && x.Approved == "N" && x.UserType == "SUP"))
            {
                UserRegisterPendingDto userPending = new UserRegisterPendingDto();
                userPending.UserId = user.UserId;
                userPending.Name = user.UserName + " " + user.Lastname;
                userPending.SupCode = (user.SupCode != null ? user.SupCode : ' ') + "-" + supGroups.FirstOrDefault(x => x.InternalSupGroupId == user.InternalSupGroupId).SupName;

                userPendings.Add(userPending);
            }

            await _hub.Clients.All.SendAsync("userregister", userPendings);

            //var result = _hub.Clients.All.SendAsync("userregister", 5);
            return OkResponse(new { Status = "Send To Graph 1 Completed" });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await _userMasterService.GetuserById(userId);

            return OkResponse(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.UserRegister(userRegister);

                var userLists = await _userMasterService.GetUsers();

                var userList = userLists.Where(x => x.Approved != null && x.Approved == "N" && x.UserType == "SUP");


                var supGroups = await _supplierGroupService.GetSupplierGroup();

                List<UserRegisterPendingDto> userPendings = new List<UserRegisterPendingDto>();

                foreach (var user in userList)
                {
                    UserRegisterPendingDto userPending = new UserRegisterPendingDto();
                    userPending.UserId = user.UserId;
                    userPending.Name = user.UserName + " " + user.Lastname;
                    userPending.SupCode = (user.SupCode != null ? user.SupCode : ' ') + "-" + supGroups.FirstOrDefault(x => x.InternalSupGroupId == user.InternalSupGroupId).SupName;

                    userPendings.Add(userPending);
                }

                await _hub.Clients.All.SendAsync("userregister", userPendings);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create(UserMasterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.CreateUser(userRegister);

                //var userList = await _userMasterService.GetUsers();

                //await _hub.Clients.All.SendAsync("userregister", userList.Count(x=>x.Approved.ToUpper() == "N"));
                
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserRegisterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.UserRegister(userRegister);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpPost("Resetpassword")]
        public async Task<ActionResult> ResetPassword(UserRegisterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.ResetPassword(userRegister);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("forgotpassword")]
        public async Task<ActionResult> ForgotPassword(UserRegisterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.ForgotPassword(userRegister);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpGet("translate")]
        public IEnumerable<Translate> GetTranslate()
        {
            var translates = new List<Translate>();

            translates.Add(new Translate { Code = "en", Name = "en" });
            translates.Add(new Translate { Code = "th", Name = "th" });

            return translates;                
        }

        [HttpPut()]
        public async Task<ActionResult> Update(UserMasterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.UpdateUser(userRegister);
                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("sendEmail")]
        public async Task<ActionResult> SendEmail(string to,string subject,string body)
        {
            try
            {
                Email.SendEmail(to, subject, body);

                var result = "";

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

    }
}
