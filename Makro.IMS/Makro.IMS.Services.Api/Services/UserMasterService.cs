using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace Makro.IMS.Services.Api.Services
{
    public class UserMasterService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;

        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        private SupplierGroup supGroup;

        public UserMasterService(
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);

            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<List<UserMaster>> GetUsers()
        {
            return await Task.Run<List<UserMaster>>(() => unitOfWork.UserMasterRepository.GetUsers().ToList());
        }

        public async Task<UserMasterDto> GetuserById(string userId)
        {
            var user = await Task.Run<UserMaster>(() => unitOfWork.UserMasterRepository.GetUserMasterByUserId(userId));

            var userDto = new UserMasterDto();

            userDto.UserId = user.UserId;
            userDto.UserName = user.UserName;
            userDto.Password = user.Password;
            userDto.Lastname = user.Lastname;
            userDto.Approved = user.Approved;
            userDto.Admin = user.Admin;
            userDto.Email = user.Email;
            userDto.WarehouseCode = user.WarehouseCode;
            userDto.InternalSupGroupId = user.InternalSupGroupId;
            userDto.UserType = user.UserType;
            userDto.OperationType = user.OperationType;
            userDto.SupCode = user.SupCode;

            userDto.userWarehouses = new List<UserWarehouseDto>();
            userDto.userSupplierGroups = new List<UserSupplierGroupDto>();

            var userWarehouse = unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(userId).ToList();

            foreach (var item in userWarehouse)
            {
                var userWhse = new UserWarehouseDto();
                userWhse.WarehouseCode = item.WarehouseCode;
                userDto.userWarehouses.Add(userWhse);
            }

            var userSupGroup = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(userId).ToList();
            foreach (var sup in userSupGroup)
            {
                var userSup = new UserSupplierGroupDto();
                userSup.InternalSupGroupId = sup.InternalSupGroup;
                userDto.userSupplierGroups.Add(userSup);
            }

            return userDto;

        }

        public async Task<UserResponse> Login(LoginDto loginDto)
        {

            try
            {
                var password = Helper.EnCry.Encrypt(loginDto.Password, true);

                var users = imsContext.UserMasters.Where(x => x.UserId == loginDto.UserName && x.Password == password && x.Approved.ToUpper() == "Y");

                var user = users.Count() > 0 ? users.ToList()[0] : null;

                if (user == null)
                {
                    throw new Exception("Login failed");
                }

               

                var userClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserId)
                };

                userClaims.Add(new Claim(ClaimTypes.Role, user.UserType));

                var identity = _jwtFactory.GenerateClaimsIdentity(userClaims, user.UserId, user.UserId);
                var refreshToken = await Tokens.GenerateRefreshToken();

                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, loginDto.UserName, refreshToken, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

                UserResponse? userResponse = SetUserData(user, jwt);

                user.Token = userResponse.AccessToken;

                imsContext.UserMasters.Update(user);
                imsContext.SaveChanges();

                return userResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        private UserResponse SetUserData(UserMaster? user, string jwt)
        {
            var userResponse = JsonConvert.DeserializeObject<UserResponse>(jwt);

            userResponse.UserInfo = new UserDto();
            userResponse.UserInfo.UserName = user.UserName;
            userResponse.UserInfo.UserId = user.UserId;
            userResponse.UserInfo.Name = user.UserName;
            userResponse.UserInfo.LastName = user.Lastname;
            userResponse.UserInfo.InternalSupGroupId = user.InternalSupGroupId;
            userResponse.UserInfo.UserType = user.UserType;
            userResponse.UserInfo.WarehouseCode = user.WarehouseCode;
            userResponse.UserInfo.OperationType = user.OperationType;
            userResponse.UserInfo.UserWarehouses = new List<UserWarehouseDto>();
            userResponse.UserInfo.UserSupplierGroups = new List<UserSupplierGroupDto>();

            if (user.ExpireDate.Value.Date <= DateTime.Now.Date)
            {
                userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                userResponse.UserInfo.AuthMenus = ExpireMenu();
            }
            else
            {

                if (user.UserType.ToLower() == "sup" || user.UserType.ToLower() == "suptran")
                {
                    userResponse.UserInfo.SupCode = user.InternalSupGroupId.Value.ToString();
                    userResponse.UserInfo.SupGroup = unitOfWork.SupplierGroupRepository.GetSupplierGroupById(Convert.ToInt32(user.InternalSupGroupId.Value));

                    supGroup = userResponse.UserInfo.SupGroup;
                }

                if(userResponse.UserInfo.WarehouseCode != null)
                {
                    userResponse.UserInfo.UserWarehouses.Add(new UserWarehouseDto { WarehouseCode = user.WarehouseCode });
                }

                if(userResponse.UserInfo.SupCode != null)
                {
                    userResponse.UserInfo.UserSupplierGroups.Add(new UserSupplierGroupDto { InternalSupGroupId = user.InternalSupGroupId.Value });
                }

                userResponse.UserInfo.UserWarehouses.AddRange(unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(user.UserId).Select(x => new UserWarehouseDto
                {
                    WarehouseCode = x.WarehouseCode
                }).ToList());

                userResponse.UserInfo.UserSupplierGroups.AddRange(unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(user.UserId).Select(x => new UserSupplierGroupDto
                {
                    InternalSupGroupId = x.InternalSupGroup
                }).ToList());

                var AuthMenus = new List<AuthMenu>();

                if (user.UserType != null)
                {
                    switch (user.UserType.ToLower())
                    {
                        case "develop":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = DevMenu();
                            break;
                        case "admin":
                        case "dhl":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = DhlMenu();
                            break;
                        case "ops":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = OpsMenu();
                            break;
                        case "control":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = ControlMenu();
                            break;
                        case "sec":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = SecurityMenu();
                            break;
                        case "data":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = DataMenu();
                            break;
                        case "sup":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = SupMenu();
                            break;
                        case "suptran":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = SupTranMenu();
                            break;
                        case "driver":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = DriverMenu();
                            break;
                        case "report":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = ReportMenu();
                            break;
                        case "sfsadmin":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = SFSAdmin();
                            break;
                        case "superuser":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = SuperUserMenu();
                            break;
                        case "yard":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = YardMenu();
                            break;
                        case "waive":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = WaiveMenu();
                            break;
                        case "bh":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = BhMenu();
                            break;
                        case "transport":
                            userResponse.UserInfo.AuthMenus = new List<AuthMenu>();
                            userResponse.UserInfo.AuthMenus = TransportTeamMenu();
                            break;
                        default:
                            break;
                    }

                }
            }

            userResponse.UserInfo.Translate = "en";
            return userResponse;
        }

        public async Task<bool> CreateUser(UserMasterDto user)
        {

            try
            {
                //var user = new UserMaster();

                var password = Helper.EnCry.Encrypt(user.Password.Trim(), true);

                var users = unitOfWork.UserMasterRepository.GetUserMasterByUserId(user.UserId.Trim());

                if (users != null)
                {
                    throw new Exception("Register failed : Existing user login");
                }

                user.Password = password;
                if (user.UserType == "ADMIN")
                {
                    user.Admin = true;
                }
                else
                {
                    user.Admin = false;
                }
                user.UserId = user.UserId.Trim();

                var newUser = new UserMaster();
                newUser.UserId = user.UserId;
                newUser.UserName = user.UserName;
                newUser.Lastname = user.Lastname;
                newUser.Password = user.Password;
                newUser.UserType = user.UserType;
                newUser.Admin = user.Admin;
                newUser.Email = user.Email;
                newUser.InternalSupGroupId = user.InternalSupGroupId;
                newUser.WarehouseCode = user.WarehouseCode;
                newUser.Approved = user.Approved;
                newUser.OperationType = user.OperationType;
                newUser.CreateDate = DateTime.Now;
                newUser.ModDate = DateTime.Now;
                newUser.UserStamp = "register";
                newUser.ExpireDate = DateTime.Now.AddDays(90);
                newUser.SupCode = user.SupCode;

                unitOfWork.UserMasterRepository.AddUser(newUser);

                foreach (var userWarehouse in user.userWarehouses)
                {
                    var userWhse = new UserWarehouse();
                    userWhse.WarehouseCode = userWarehouse.WarehouseCode;
                    userWhse.UserId = user.UserId;
                    unitOfWork.UserWarehouseRepository.AddUserWarehouse(userWhse);
                }

                foreach (var userSupGrp in user.userSupplierGroups)
                {
                    var userSup = new UserSupplierGroup();
                    userSup.InternalSupGroup = Convert.ToInt32(userSupGrp.InternalSupGroupId);
                    userSup.UserId = user.UserId;
                    unitOfWork.UserSupplierGroupRepository.AddUserSupplierGroup(userSup);
                }

                unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<bool> UpdateUser(UserMasterDto user)
        {
            try
            {
                var password = Helper.EnCry.Encrypt(user.Password.Trim(), true);

                var curUser = unitOfWork.UserMasterRepository.GetUserMasterByUserId(user.UserId.Trim());

                if (curUser == null)
                {
                    throw new Exception("Update failed : User not found");
                }

                curUser.InternalSupGroupId = user.InternalSupGroupId;
                curUser.Approved = user.Approved;
                curUser.UserName = user.UserName;
                curUser.Lastname = user.Lastname;
                curUser.Email = user.Email;
                curUser.OperationType = user.OperationType;
                curUser.WarehouseCode = user.WarehouseCode;
                curUser.ModDate = DateTime.Now;
                curUser.UserType = user.UserType;
                curUser.InternalSupGroupId = user.InternalSupGroupId;
                curUser.SupCode = user.SupCode;

                if (user.UserType == "ADMIN")
                {
                    curUser.Admin = true;
                }
                else
                {
                    curUser.Admin = false;
                }

                unitOfWork.UserMasterRepository.UpdateUser(curUser);

                // clear all userWarehouse
                var userWhses = unitOfWork.UserWarehouseRepository.GetUserWarehouseByUserId(user.UserId).ToList();
                foreach (var item in userWhses)
                {
                    unitOfWork.UserWarehouseRepository.DeleteUserWarehouse(item);
                }

                foreach (var userWarehouse in user.userWarehouses)
                {
                    var userWhse = new UserWarehouse();
                    userWhse.WarehouseCode = userWarehouse.WarehouseCode;
                    userWhse.UserId = user.UserId;
                    unitOfWork.UserWarehouseRepository.AddUserWarehouse(userWhse);
                }

                // clear all userSupplierGroup
                var userSupGroups = unitOfWork.UserSupplierGroupRepository.GetUserSupplierByUserId(user.UserId).ToList();
                foreach (var item in userSupGroups)
                {
                    unitOfWork.UserSupplierGroupRepository.DeleteUserSupplierGroup(item);
                }

                foreach (var userSupGrp in user.userSupplierGroups)
                {
                    var userSup = new UserSupplierGroup();
                    userSup.InternalSupGroup = Convert.ToInt32(userSupGrp.InternalSupGroupId);
                    userSup.UserId = user.UserId;
                    unitOfWork.UserSupplierGroupRepository.AddUserSupplierGroup(userSup);
                }

                unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> NewPassword(UserMaster user)
        {
            try
            {
                var password = Helper.EnCry.Encrypt(user.Password.Trim(), true);

                var curUser = unitOfWork.UserMasterRepository.GetUserMasterByUserId(user.UserId.Trim());

                if (curUser == null)
                {
                    throw new Exception("Update failed : User not found");
                }

                curUser.Password = password;
                curUser.ModDate = DateTime.Now;
                curUser.UserType = user.UserType;
                curUser.ExpireDate = DateTime.Now.AddDays(90);

                unitOfWork.UserMasterRepository.UpdateUser(curUser);
                unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UserRegister(UserRegisterDto userRegister)
        {

            try
            {
                var user = new UserMaster();

                var password = Helper.EnCry.Encrypt(userRegister.Password.Trim(), true);

                var users = imsContext.UserMasters.Where(x => x.UserId == userRegister.UserName.Trim()).ToList();

                if (users != null && users.Count > 0)
                {
                    throw new Exception("Register failed : Existing user login");
                }

                var supplier = imsContext.Suppliers.FirstOrDefault(x => x.SupCode == userRegister.SupCode);

                if (supplier == null)
                {
                    throw new Exception("Register failed : Please check sup code");
                }
                else
                {
                    if (supplier.InternalGroupId <= 0)
                    {
                        throw new Exception("Register failed : ติดต่อ booking team");
                    }
                }


                user.UserId = userRegister.UserName.Trim();
                user.UserName = userRegister.FirstName;
                user.Lastname = userRegister.LastName;
                user.Password = password;
                user.UserType = "SUP";
                user.Admin = false;
                user.CreateDate = DateTime.Now;
                user.ModDate = DateTime.Now;
                user.ExpireDate = DateTime.Now;
                user.Approved = "N";
                user.UserStamp = "register";
                user.InternalSupGroupId = supplier.InternalGroupId;
                user.Email = userRegister.Email;
                user.ExpireDate = DateTime.Now.AddDays(90);
                user.SupCode = userRegister.SupCode;

                imsContext.Add(user);
                imsContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<bool> Delete(UserMaster userRegister)
        {

            try
            {

                var user = imsContext.UserMasters.FirstOrDefault(x => x.UserId == userRegister.UserId);


                imsContext.UserMasters.Remove(user);
                imsContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<bool> ResetPassword(UserRegisterDto userRegister)
        {

            try
            {
                var user = new UserMaster();

                var password = Helper.EnCry.Encrypt(userRegister.Password.Trim(), true);

                var users = imsContext.UserMasters.Where(x => x.UserId == userRegister.UserName.Trim()).ToList();

                if (users != null && users.Count > 0)
                {
                    if (password == users[0].Password)
                    {
                        throw new Exception("Cannot use this password.");
                    }

                    user = users[0];
                    user.Password = password;
                    user.ExpireDate = DateTime.Now.AddDays(45);
                    user.ModDate = DateTime.Now;
                    user.UserStamp = user.UserId;
                }

                imsContext.Update(user);
                imsContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<UserResponse> RefreshToken(TokenModel tokenModel)
        {
            try
            {
                var users = imsContext.UserMasters.Where(x => x.UserId == tokenModel.UserInfo.UserId && x.Approved.ToUpper() == "Y");

                var user = users.Count() > 0 ? users.ToList()[0] : null;

                if (user == null)
                {
                    throw new Exception("Login failed");
                }

                var userClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserId)
                };

                userClaims.Add(new Claim(ClaimTypes.Role, user.UserType));

                var identity = _jwtFactory.GenerateClaimsIdentity(userClaims, user.UserId, user.UserId);
                var refreshToken = await Tokens.GenerateRefreshToken();

                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, user.UserName, refreshToken, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

                UserResponse? userResponse = SetUserData(user, jwt);

                user.Token = userResponse.AccessToken;

                imsContext.UserMasters.Update(user);
                imsContext.SaveChanges();


                return userResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<AuthMenu> DevMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Dashboard
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            // Master data
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Warehouse", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "OperationType", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Door", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "Master", PageName = "WarehouseCapacity", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "WarehouseOperationCapacity", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "SupplierGroup", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Supplier", Action = "Modify" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            // Check-In            
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckOut", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCreateBooking", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "ManageQueue", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "GuardCreateBooking", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentConfirmation", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "OpsManage", PageName = "DockDoorList", Action = "Modify" });

            //   authMenus.Add(new AuthMenu() { Module = "Yard", PageName = "YardBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Yard", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Shunt", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Trailer", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "YardMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TrailerMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerEmpty", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerFull", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferList", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferProcess", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferReport", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserMaster", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> DhlMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Dashboard
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            // Master data
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Warehouse", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "OperationType", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Door", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "Master", PageName = "WarehouseCapacity", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "Master", PageName = "WarehouseOperationCapacity", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "SupplierGroup", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Supplier", Action = "Modify" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            // Check-In            
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckOut", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCreateBooking", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "ManageQueue", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "GuardCreateBooking", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentConfirmation", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "OpsManage", PageName = "DockDoorList", Action = "Modify" });

            //   authMenus.Add(new AuthMenu() { Module = "Yard", PageName = "YardBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Yard", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Shunt", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Trailer", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "YardMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TrailerMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerEmpty", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerFull", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferList", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferProcess", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferReport", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserMaster", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> SuperUserMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Dashboard
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "SupplierGroup", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Supplier", Action = "Modify" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            // Check-In            
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckOut", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCreateBooking", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "ManageQueue", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "GuardCreateBooking", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentConfirmation", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "OpsManage", PageName = "DockDoorList", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            
            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> SFSAdmin()
        {
            var authMenus = new List<AuthMenu>();


            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });

            // Pre Check-In            
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "ManageQueue", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "GuardCreateBooking", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> DataMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Dashboard
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "ManageQueue", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "GuardCreateBooking", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "DocumentConfirmation", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> ControlMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "SupplierGroup", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Master", PageName = "Supplier", Action = "Modify" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            // Check-In            
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckOut", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "QueueManage", PageName = "ManageQueue", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> ReportMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> OpsMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "OpsManage", PageName = "DockDoorList", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> SecurityMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Check-In            
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckIn", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCheckOut", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "CheckInOut", PageName = "GuardCreateBooking", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> SupMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "View" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });            
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CapMonitor", Action = "Modify" });

            if (supGroup != null)
            {
                if (supGroup.IsUpCreateBook.ToUpper() == "Y")
                {
                    authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
                }
            }

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            // User Account
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "SupplierContact", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> SupTranMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            // User Account
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> YardMenu()
        {
            var authMenus = new List<AuthMenu>();
            
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Yard", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Shunt", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Trailer", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "YardMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TrailerMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerEmpty", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerFull", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferList", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferProcess", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferReport", Action = "Modify" });

            // User Account
            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "SupplierContact", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> WaiveMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });
          
            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            // authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            // User Account
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> DriverMenu()
        {
            var authMenus = new List<AuthMenu>();

            // Check-In
            authMenus.Add(new AuthMenu() { Module = "Driver", PageName = "GatePass", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Driver", PageName = "Queue", Action = "Modify" });

            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "UserProfile", Action = "Modify" });
            //authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });            
            return authMenus;
        }

        private List<AuthMenu> ExpireMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> BhMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
            // authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            // User Account
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }

        private List<AuthMenu> TransportTeamMenu()
        {
            var authMenus = new List<AuthMenu>();

            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "Overview", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "ScheduleDoor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SummaryBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Dashboard", PageName = "SlottimeBooking", Action = "Modify" });

            // Booking
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "BookingHeader", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBooking", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingExcel", Action = "Modify" });
            // authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CreateBookingNopo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "CheckPo", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Booking", PageName = "PoMonitor", Action = "Modify" });

            // Pre Check-In
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInExcel", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "PreCheckIn", PageName = "PreCheckInCompleted", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TruckStatus", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Report", PageName = "TransactionTrack", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Yard", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Shunt", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "YardMaster", PageName = "Trailer", Action = "Modify" });

            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "YardMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TrailerMonitor", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerEmpty", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferTrailerFull", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferList", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferProcess", Action = "Modify" });
            authMenus.Add(new AuthMenu() { Module = "Transfer", PageName = "TransferReport", Action = "Modify" });

            // User Account
            authMenus.Add(new AuthMenu() { Module = "Account", PageName = "ResetPassword", Action = "Modify" });

            return authMenus;
        }


        // Login Driver
        public async Task<UserResponse> LoginDriver(LoginDto loginDto)
        {
            try
            {
                //var password = Helper.EnCry.Encrypt(loginDto.Password, true);
                byte[] decodedBytes = Convert.FromBase64String(loginDto.UserName);

                // Convert the byte array to a string
                string decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

                var driver = imsContext.BookingTruckCheckIns.FirstOrDefault(x => x.TelNo == decodedString);

                if (driver == null)
                {
                    throw new Exception("Login failed");
                }

                var userClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, driver.TelNo)
                };

                userClaims.Add(new Claim(ClaimTypes.Role, "Driver"));

                var identity = _jwtFactory.GenerateClaimsIdentity(userClaims, driver.TelNo, driver.TelNo);
                var refreshToken = await Tokens.GenerateRefreshToken();

                var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, loginDto.UserName, refreshToken, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

                UserMaster user = new UserMaster();
                user.UserId = driver.TelNo;
                user.UserName = driver.TelNo;
                user.ExpireDate = DateTime.Now.AddDays(10);
                user.UserType = "Driver";
                user.Approved = "1";

                UserResponse? userResponse = SetUserData(user, jwt);

                return userResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Approved(UserMaster user)
        {
            try
            {
                var curUser = imsContext.UserMasters.FirstOrDefault(x => x.UserId == user.UserId.Trim());

                if (curUser == null)
                {
                    throw new Exception("Update failed : User not found");
                }

                curUser.Approved = "Y";
                curUser.ModDate = DateTime.Now;
                curUser.UserStamp = user.UserStamp;

                imsContext.Update(curUser);
                imsContext.SaveChanges();

                string htmlContent = $@"
                <!DOCTYPE html>
                <html lang='th'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Notification Email</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;'>
                    <table style='max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='text-align: left;'>
                                <h2 style='color: #4caf50;'>เรียน {user.UserId}</h2>
                                <p style='color: #333;text-indent: 2em;'>
                                   User ที่ทำการร้องขอมาถูกอนุมัติให้เข้าใช้งานเรียบร้อยแล้ว สามารถใช้ User และ Password ที่ลงทะเบียนไว้ log in เข้าระบบได้ทันที <br>
                                   Link: <a href='https://bookingprd.siammakro.co.th/ims/'>IMS</a>
                                </p>
                               
                                <p style='color: #333;'>
                                    อีเมล์ฉบับนี้ถูกส่งออกจากระบบตอบรับอัตโนมัติ กรุณาอย่าตอบกลับมาด้วยอีเมล์ฉบับนี้<br>                                    
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                //// send email
                //StringBuilder sb = new StringBuilder();

                //sb.Append("To " + user.UserId);
                //sb.Append('\n');
                //sb.Append("User ที่ทำการร้องขอมาถูกอนุมัติให้เข้าใช้งานเรียบร้อยแล้ว สามารถใช้ User และ Password ที่ลงทะเบียนไว้ log in เข้าระบบได้ทันที");
                //sb.Append('\n');
                //sb.Append("Link: https://booking.siammakro.co.th/ims/");
                //sb.Append('\n');
                //sb.Append("อีเมล์ฉบับนี้ถูกส่งออกจากระบบตอบรับอัตโนมัติ กรุณาอย่าตอบกลับมาด้วยอีเมล์ฉบับนี้");

                // send email
                Helper.Email.SendEmail(curUser.Email, htmlContent, "CDC IMS: ผลการยืนยันการลงทะเบียนผู้ใช้งาน");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Reject(UserMaster user)
        {
            try
            {
                var curUser = imsContext.UserMasters.FirstOrDefault(x => x.UserId == user.UserId.Trim());

                if (curUser == null)
                {
                    throw new Exception("Update failed : User not found");
                }

                imsContext.Remove(curUser);
                imsContext.SaveChanges();

                string htmlContent = $@"
                <!DOCTYPE html>
                <html lang='th'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Notification Email</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;'>
                    <table style='max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='text-align: left;'>
                                <h2 style='color: #4caf50;'>เรียน {user.UserId}</h2>
                                <p style='color: #333;text-indent: 2em;'>
                                   User ที่ทำการร้องขอมาไม่สามารถอนุมัติให้เข้าใช้งานได้ <br>
                                   สาเหตุ: {user.UserName} <br>
                                   กรุณาดำเนินการ register เข้ามาใหม่ หรือติดต่อกลับทีม Booking   
                                </p>
                                
                                <p style='color: #333;'>
                                    อีเมล์ฉบับนี้ถูกส่งออกจากระบบตอบรับอัตโนมัติ กรุณาอย่าตอบกลับมาด้วยอีเมล์ฉบับนี้<br>                                    
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                //StringBuilder sb = new StringBuilder();

                //sb.Append("To " + user.UserId);
                //sb.Append('\n');
                //sb.Append("User ที่ทำการร้องขอมาไม่สามารถอนุมัติให้เข้าใช้งานได้");
                //sb.Append('\n');
                //sb.Append("สาเหตุ: " + user.UserName);
                //sb.Append('\n');
                //sb.Append("กรุณาดำเนินการ register เข้ามาใหม่ หรือติดต่อกลับทีม Booking");
                //sb.Append('\n');
                //sb.Append("อีเมล์ฉบับนี้ถูกส่งออกจากระบบตอบรับอัตโนมัติ กรุณาอย่าตอบกลับมาด้วยอีเมล์ฉบับนี้");

                // send email
                Helper.Email.SendEmail(curUser.Email, htmlContent, "CDC IMS: ผลการยืนยันการลงทะเบียนผู้ใช้งาน");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ForgotPassword(UserRegisterDto userRegister)
        {

            try
            {

                var newPassword = RandomPassword();
                var password = Helper.EnCry.Encrypt(newPassword, true);

                var user = imsContext.UserMasters.FirstOrDefault(x => x.UserId == userRegister.UserName.Trim());

                if (user == null)
                {
                    throw new Exception("Reset password failed : Not found user login");
                }

                var supplier = imsContext.Suppliers.FirstOrDefault(x => x.SupCode == userRegister.SupCode);

                if (supplier == null)
                {
                    throw new Exception("Reset password failed : Please check sup code");
                }

                user.Password = password;
                user.ModDate = DateTime.Now;
                user.UserStamp = "fotgotpassword";
                user.ExpireDate = DateTime.Now.AddDays(45);

                imsContext.Update(user);
                imsContext.SaveChanges();

                string htmlContent = $@"
                <!DOCTYPE html>
                <html lang='th'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Notification Email</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;'>
                    <table style='max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <tr>
                            <td style='text-align: left;'>
                                <h2 style='color: #4caf50;'>เรียน {user.UserId}</h2>
                                <p style='color: #333;text-indent: 2em;'>
                                   ทำการ reset รหัสผ่านเรียบร้อยแล้ว <br>
                                   สามารถเข้าใช้งานด้วยรหัสผ่าน: {newPassword} <br>
                                   กรุณาเปลี่ยนรหัสผ่านหลังจากเข้าระบบได้เรียบร้อยแล้ว  
                                </p>
                                
                                <p style='color: #333;'>
                                    อีเมล์ฉบับนี้ถูกส่งออกจากระบบตอบรับอัตโนมัติ กรุณาอย่าตอบกลับมาด้วยอีเมล์ฉบับนี้<br>                                    
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                // send email
                //StringBuilder sb = new StringBuilder();

                //sb.Append("To " + user.UserId);
                //sb.Append('\n');
                //sb.Append("ทำการ reset รหัสผ่านเรียบร้อยแล้ว");
                //sb.Append('\n');
                //sb.Append("สามารถเข้าใช้งานด้วยรหัสผ่าน: " + newPassword);
                //sb.Append('\n');
                //sb.Append("กรุณาเปลี่ยนรหัสผ่านหลังจากเข้าระบบได้เรียบร้อยแล้ว");
                //sb.Append('\n');
                //sb.Append("อีเมล์ฉบับนี้ถูกส่งออกจากระบบตอบรับอัตโนมัติ กรุณาอย่าตอบกลับมาด้วยอีเมล์ฉบับนี้");

                // send email
                Helper.Email.SendEmail(user.Email, htmlContent, "CDC IMS: ผลการยืนยันการกุ้คืนผู้ใช้งาน");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        // Generates a random password.
        // 4-LowerCase + 4-Digits + 2-UpperCase
        public string RandomPassword()
        {
            var passwordBuilder = new StringBuilder();

            // 4-Letters lower case
            passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case
            passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }

        private readonly Random _random = new Random();

        // Generates a random number within a range.
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        // Generates a random string with a given size.
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.

            // char is a single Unicode character
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public async Task<bool> ValidateToken(TokenModel token)
        {
            var user = unitOfWork.UserMasterRepository.GetUserMasterByUserId(token.UserInfo.UserId);

            if (user.Token == token.AccessToken)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
