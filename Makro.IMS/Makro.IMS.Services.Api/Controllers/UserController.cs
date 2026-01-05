using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makro.IMS.Services.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class UserController : ApiControllerBase
    {
        private readonly UserMasterService _userMasterService;
        private readonly SupplierGroupService _supplierGroupService;
        

        public UserController(
            [FromServices] UserMasterService userMasterService,
            [FromServices] SupplierGroupService supplierGroupService
            )
        {
            _userMasterService = userMasterService;
            _supplierGroupService = supplierGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userMasterService.GetUsers();

            return OkResponse(result);
        }

        [HttpGet("pendinguser")]
        public async Task<IActionResult> GetPendingUser()
        {
            var result = await _userMasterService.GetUsers();
            var userList = result.Where(x => x.Approved != null && x.Approved == "N" && x.UserType == "SUP");

            var supGroups = await _supplierGroupService.GetSupplierGroup();

            List<UserRegisterPendingDto> userPendings = new List<UserRegisterPendingDto>();

            foreach (var user in userList)
            {
                UserRegisterPendingDto userPending = new UserRegisterPendingDto();
                userPending.UserId = user.UserId;
                userPending.Name = user.UserName + " " + user.Lastname;
                userPending.SupCode = (user.SupCode != null ? user.SupCode  : ' ') + "-" + supGroups.FirstOrDefault(x => x.InternalSupGroupId == user.InternalSupGroupId).SupName;

                userPendings.Add(userPending);
            }

            return OkResponse(userPendings);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await _userMasterService.GetuserById(userId);

            return OkResponse(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create(UserMasterDto userRegister)
        {
            try
            {
                var result = await _userMasterService.CreateUser(userRegister);                
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

        [HttpPost("newpassword")]
        public async Task<ActionResult> NewPassword(UserMaster user)
        {
            try
            {
                var result = await _userMasterService.NewPassword(user);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpPost("approved")]
        public async Task<ActionResult> ApprovedUser(UserMaster user)
        {
            try
            {
                var result = await _userMasterService.Approved(user);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }


        [HttpPost("reject")]
        public async Task<ActionResult> RejectUser(UserMaster user)
        {
            try
            {
                var result = await _userMasterService.Reject(user);

                return OkResponse(result);
            }
            catch (Exception ex)
            {
                return BadResponse(ex.Message);
            }
        }

        [HttpPost("delete")]
        public async Task<ActionResult> Delete(UserMaster userRegister)
        {
            try
            {
                var result = await _userMasterService.Delete(userRegister);
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

        [HttpPost("checksession")]
        public async Task<ActionResult> CheckSession(TokenModel token)
        {
            try
            {
                //var result = await _userMasterService.CreateUser(userRegister);
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
