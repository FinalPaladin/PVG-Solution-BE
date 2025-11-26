using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.TokenService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.AuthTokenRepository;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.UserPermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.UserService
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository,
            IMapper mapper,
            IPermissionRepository permissionRepository,
            IUserPermissionRepository userPermissionRepository,
            IAuthTokenRepository authTokenRepository,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _authTokenRepository = authTokenRepository;
        }

        public async Task<BaseResponse> Login(RQ_UserLoginModel _input)
        {
            try
            {
                if (_input == null || string.IsNullOrEmpty(_input.UserName) || string.IsNullOrEmpty(_input.Password))
                    return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_INVALID_INPUT, "Dữ liệu đầu vào không hợp lệ");

                var user = await _userRepository.FindByCondition(x => x.UserName == _input.UserName).FirstOrDefaultAsync();
                if (user == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_USER_NOT_FOUND, "Tài khoản không tồn tại");

                if (!user.Actived)
                    return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_USER_INACTIVE, "Tài khoản đã bị khóa");

                var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.Password, _input.Password);
                if (verifyPassword == PasswordVerificationResult.Failed)
                    return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_PASSWORD_WRONG, "Mật khẩu không đúng");

                var oldToken = await _authTokenRepository.FindByCondition(c => c.UserId == user.Id).ToListAsync();
                if (oldToken?.Count > 0)
                    await _authTokenRepository.DeleteListAsync(oldToken);

                var newToken = await _tokenService.CreateTokenAsync(user);

                return SuccessResponse(new
                {
                    Token = newToken,
                    FullName = user.UserName,
                    ExpireAt = DateTime.UtcNow.AddDays(30),
                });
            }
            catch (Exception ex)
            {
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> Logout(string userName)
        {
            try
            {
                var user = await _userRepository.FindByCondition(x => x.UserName == userName).FirstOrDefaultAsync();
                if (user == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_USER_NOT_FOUND, "Tài khoản không tồn tại");

                var authTokens = await _authTokenRepository.FindByCondition(x => x.UserId == user.Id).ToListAsync();
                if (authTokens == null || authTokens.Count == 0)
                    return BadRequestResponse(ErrorCodeConst.ERROR_SESSION_NOT_FOUND, "Không tìm thấy phiên đăng nhập");

                await _authTokenRepository.DeleteListAsync(authTokens);
                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> CreateUserAsync(string userName, string password, string fullName)
        {
            // Check trùng userName
            var exists = await _userRepository.FindByCondition(u => u.UserName == userName).FirstOrDefaultAsync();
            if (exists != null)
                return BadRequestResponse(ErrorCodeConst.ERROR_REGISTER_ACCOUNT_EXISTED, "Tài khoản đã tồn tại");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                FullName = fullName,
                Actived = true,
                CreatedDate = DateTime.UtcNow,
                CreatedByName = "system",
                ModifiedByName = "system"
            };

            // Hash password theo chuẩn Identity
            user.Password = _passwordHasher.HashPassword(user, password);

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

            return SuccessResponse(true);
        }

        public async Task<BaseResponse> ChangePasswordAsync(string userName, string currentPassword, string newPassword)
        {
            var user = await _userRepository.FindByCondition(u => u.UserName == userName).FirstOrDefaultAsync();
            if (user == null)
                return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_USER_NOT_FOUND, "Tài khoản không tồn tại");

            var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.Password, currentPassword);
            if (verifyPassword == PasswordVerificationResult.Failed)
                return BadRequestResponse(ErrorCodeConst.ERROR_LOGIN_PASSWORD_WRONG, "Mật khẩu hiện tại không đúng");

            user.Password = _passwordHasher.HashPassword(user, newPassword);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return SuccessResponse(true);
        }

        public async Task<BaseResponse<RS_SearchUserModel>> Search(RQ_SearchUserModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_SearchUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                IQueryable<User> query = _userRepository.FindByCondition(x => x.Actived
                    && (string.IsNullOrEmpty(_input.FullName) || x.FullName.Contains(_input.FullName))
                    && (string.IsNullOrEmpty(_input.UserName) || x.UserName.Contains(_input.UserName))
                    && x.Actived == _input.Actived
                ).AsQueryable();

                var pagination = await _userRepository.OffsetPagination<User>(query, _input.Page, _input.PageSize);

                if (pagination.Items == null)
                    return new BaseResponse<RS_SearchUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Không tìm thấy"
                    };

                var data = _mapper.Map<List<UserModel>>(pagination.Items);
                return new BaseResponse<RS_SearchUserModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tìm kiếm thành công",
                    Result = new()
                    {
                        Data = new()
                        {
                            Items = data,
                            PageNumber = pagination.PageNumber,
                            PerPage = pagination.PerPage,
                            TotalItems = pagination.TotalItems,
                            TotalPages = pagination.TotalPages,
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_SearchUserModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse<RS_GetUserModel>> Get(RQ_GetUserModel _input)
        {
            try
            {
                if (_input == null)
                    return new BaseResponse<RS_GetUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };

                if (string.IsNullOrEmpty(_input.UserName))
                    return new BaseResponse<RS_GetUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Tài khoản không tồn tại"
                    };

                var usersEntity = _userRepository.FindByCondition(x => x.UserName == _input.UserName).ToList();

                if (usersEntity == null)
                    return new BaseResponse<RS_GetUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Tài khoản không tồn tại"
                    };

                var data = _mapper.Map<UserModel>(usersEntity);
                return new BaseResponse<RS_GetUserModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy dữ liệu thành công",
                    Result = new()
                    {
                        Data = data
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetUserModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse> Delete(RQ_DeleteUserModel _input)
        {
            try
            {
                var userDeleteEntity = await _userRepository.FindByCondition(x => x.UserName == _input.DeleteUser).FirstOrDefaultAsync();

                if (userDeleteEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Tài khoản không tồn tại",
                    };
                }

                var AD = UserAdminType.SystemAdmin;

                var isAdmin = await CheckAdmin(_input.UserAction, AD);

                if (isAdmin == null || !isAdmin.IsSuccess)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = string.Format("Phải là {0} mới đủ quyền xóa", nameof(AD)),
                    };
                }

                await _userRepository.DeleteAsync(userDeleteEntity);
                await _userRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Delete completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<UserModel>> CheckAdmin(string _userName, UserAdminType _adminType)
        {
            try
            {
                string systemAD = nameof(_adminType);

                var userEntity = _userRepository.FindByCondition(x => x.Actived && x.UserName == _userName).FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new BaseResponse<UserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Not admin",
                        Result = null
                    };
                }

                var user = _mapper.Map<UserModel>(userEntity);

                //var permissionEntity = _permissionRepository.FindByCondition(x => x.Name == systemAD && x.IsDeleted == false).FirstOrDefaultAsync();

                //if (permissionEntity == null)
                //{
                //    return new BaseResponse<UserModel>()
                //    {
                //        IsSuccess = false,
                //        StatusCode = StatusCodes.Status404NotFound,
                //        Message = "Not admin",
                //        Result = user
                //    };
                //}

                return new BaseResponse<UserModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Admin",
                    Result = user
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }
    }
}