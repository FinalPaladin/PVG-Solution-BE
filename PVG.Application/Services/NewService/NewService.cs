using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.PermissionService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.NewRepository;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static PVG.Domain.Enums.NewEnum;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.NewService
{
    public class NewService : INewService
    {
        private readonly INewRepository _newRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        public NewService(INewRepository newRepository,
            IMapper mapper,
            IUserService userService,
            IUserRepository userRepository)
        {
            _newRepository = newRepository;
            _mapper = mapper;
            _userService = userService;
            _userRepository = userRepository;
        }


        public async Task<BaseResponse> Save(RQ_SaveNewModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Điều kiện nhập trống"
                    };
                }

                var id = Guid.NewGuid();

                var dataUpdate = await _newRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                var userEntity = await _userRepository.FindByCondition(x => x.UserName == _input.CreateUser && x.IsDeleted == false).FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Người dùng không tồn tại",
                    };
                }

                if (dataUpdate != null)
                {
                    dataUpdate.ModifiedBy = userEntity.Id;
                    dataUpdate.ModifiedByName = userEntity.FullName;
                    dataUpdate.ModifiedDate = DateTime.Now;

                    dataUpdate.Title = _input.Title;
                    dataUpdate.Content = _input.Content;
                    dataUpdate.PostedDate = _input.PostedDate;
                    dataUpdate.Status = _input.Status;
                    await _newRepository.UpdateAsync(dataUpdate);
                }
                else
                {
                    var dataCreate = new New()
                    {
                        CreatedBy = userEntity.Id,
                        CreatedByName = userEntity.FullName,
                        CreatedDate = DateTime.Now,
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,

                        Title = _input.Title,
                        Content = _input.Content,
                        PostedDate = _input.PostedDate,
                        Status = Domain.Enums.NewEnum.NewStatus.Created
                    };
                    await _newRepository.CreateAsync(dataCreate);
                }

                await _newRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lưu dữ liệu thành công",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<RS_SearchNewModel>> Search(RQ_SearchNewModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_SearchNewModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                IQueryable<New> query = _newRepository.FindByCondition(x => x.IsDeleted == false
                    && (string.IsNullOrEmpty(_input.Title) || x.Title.Contains(_input.Title))
                    && (string.IsNullOrEmpty(_input.Content) || x.Content.Contains(_input.Content))
                    && x.Status == _input.Status
                    && (_input.PostedDate == null || x.PostedDate == _input.PostedDate)
                ).AsQueryable();

                var pagination = await _newRepository.OffsetPagination<New>(query, _input.Page, _input.PageSize);

                var data = _mapper.Map<List<NewModel>>(pagination.Items);

                return new BaseResponse<RS_SearchNewModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lấy dữ liệu thành công",
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
                return new BaseResponse<RS_SearchNewModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<RS_GetNewModel>> Get(RQ_GetNewModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_GetNewModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Điều kiện nhập trống"
                    };
                }

                var productEntity = await _newRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                var data = _mapper.Map<NewModel>(productEntity);

                return new BaseResponse<RS_GetNewModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lấy dữ liệu thành công",
                    Result = new()
                    {
                        Data = data
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetNewModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> Delete(RQ_DeleteNewModel _input)
        {
            try
            {

                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Điều kiện nhập trống"
                    };
                }

                var tableEntity = await _newRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                if (tableEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu không tồn tại"
                    };
                }

                var AD = tableEntity?.Status == NewStatus.Created ? UserAdminType.MarketingAdmin : UserAdminType.SystemAdmin;

                var isAdmin = await _userService.CheckAdmin(_input.UserDelete, AD);

                if (isAdmin == null || !isAdmin.IsSuccess)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = string.Format("Phải là {0} mới đủ quyền xóa", nameof(AD)),
                    };
                }

                tableEntity.IsDeleted = true;
                tableEntity.DeletedDate = DateTime.Now;
                tableEntity.DeletedBy = isAdmin.Result.Id;

                await _newRepository.UpdateAsync(tableEntity);
                await _newRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Xóa dữ liệu thành công"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }
    }
}
