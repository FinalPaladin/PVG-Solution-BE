using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.UserPermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using PVG.Infrastucture.Repositories.ViewLogRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;

        public UserService(IUserRepository userRepository,
            IMapper mapper,
            IPermissionRepository permissionRepository,
            IUserPermissionRepository userPermissionRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task<BaseResponse> Login(RQ_UserLoginModel _input)
        {
            try
            {
                if(_input == null)
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Input empty"
                    };

                if(string.IsNullOrEmpty(_input.UserName) || string.IsNullOrEmpty(_input.Password))
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Wrong username/password"
                    };

                var data = _userRepository.FindByCondition(x => x.UserName == _input.UserName && x.Password == _input.Password).FirstOrDefault();

                if(data == null)
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Wrong username/password"
                    };

                if(!data.Actived)
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "User locked"
                    };

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login successed"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse<RS_GetAllUserModel>> GetAllData()
        {
            try
            {
                var usersEntity = _userRepository.FindAll().ToList();

                if(usersEntity == null)
                    return new BaseResponse<RS_GetAllUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Data not found"
                    };

                var data = _mapper.Map<List<UserModel>>(usersEntity);
                return new BaseResponse<RS_GetAllUserModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get data successed",
                    Result = new()
                    {
                        Data = data
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetAllUserModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse<RS_GetUserModel>> GetUser(RQ_GetUserModel _input)
        {
            try
            {
                if (_input == null)
                    return new BaseResponse<RS_GetUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Input empty"
                    };

                if (string.IsNullOrEmpty(_input.UserName))
                    return new BaseResponse<RS_GetUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "User does not exist"
                    };

                var usersEntity = _userRepository.FindByCondition(x => x.UserName == _input.UserName).ToList();

                if (usersEntity == null)
                    return new BaseResponse<RS_GetUserModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Data not found"
                    };

                var data = _mapper.Map<UserModel>(usersEntity);
                return new BaseResponse<RS_GetUserModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get data successed",
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
                var userDeleteEntity = await _userRepository.FindByCondition(x => x.UserName == _input.UserDelete).FirstOrDefaultAsync();

                if (userDeleteEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Tài khoản không tồn tại",
                    };
                }

                var userActionEntity = await _userRepository.FindByCondition(x => x.UserName == _input.UserAction).FirstOrDefaultAsync();

                if (userActionEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Tài khoản không tồn tại",
                    };
                }

                var userAction = _mapper.Map<UserModel>(userActionEntity);

                var permissionEntity = _permissionRepository.FindByCondition(x => x.Name == "DELETE_REQUEST_CUSTOMER").FirstOrDefaultAsync();

                if (permissionEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                var permission = _mapper.Map<PermissionModel>(permissionEntity);

                var userPermissionEntity = _userPermissionRepository.FindByCondition(x => x.UserId == userAction.Id && x.PermissionId == permission.Id).FirstOrDefaultAsync();

                if (userPermissionEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
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
    }
}
