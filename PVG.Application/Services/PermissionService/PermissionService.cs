using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.PermissionService
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public PermissionService(IPermissionRepository permissionRepository,
            IMapper mapper,
            IUserService userService,
            IUserRepository userRepository)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _userService = userService;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse> Save(RQ_SavePermissionModel _input)
        {
            return new();
            //try
            //{
            //    if (_input == null)
            //    {
            //        return new BaseResponse()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status400BadRequest,
            //            Message = "Điều kiện nhập trống"
            //        };
            //    }

            //    var id = Guid.NewGuid();

            //    var dataUpdate = await _permissionRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

            //    var userEntity = await _userRepository.FindByCondition(x => x.Id == _input.CreateUserId).FirstOrDefaultAsync();

            //    if (userEntity == null)
            //    {
            //        return new BaseResponse()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status404NotFound,
            //            Message = "Người dùng không tồn tại",
            //        };
            //    }

            //    if (dataUpdate != null)
            //    {
            //        dataUpdate. = _input.Name;
            //        dataUpdate.ModifiedBy = userEntity.Id;
            //        dataUpdate.ModifiedByName = userEntity.FullName;
            //        dataUpdate.ModifiedDate = DateTime.Now;
            //        await _permissionRepository.UpdateAsync(dataUpdate);
            //    }
            //    else
            //    {
            //        var dataCreate = new Permission()
            //        {
            //            CreatedBy = userEntity.Id,
            //            CreatedByName = userEntity.FullName,
            //            CreatedDate = DateTime.Now,
            //            ModifiedBy = null,
            //            ModifiedByName = "",
            //            ModifiedDate = DateTime.Now,

            //            Name = _input.Name,
            //        };
            //        await _permissionRepository.CreateAsync(dataCreate);
            //    }

            //    await _permissionRepository.SaveChangesAsync();

            //    return new BaseResponse()
            //    {
            //        IsSuccess = true,
            //        StatusCode = StatusCodes.Status404NotFound,
            //        Message = "Lưu dữ liệu thành công",
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new BaseResponse()
            //    {
            //        IsSuccess = false,
            //        StatusCode = StatusCodes.Status200OK,
            //        Message = ex.Message,
            //    };
            //}
        }

        public async Task<BaseResponse<RS_SearchPermissionModel>> Search(RQ_SearchPermissionModel _input)
        {
            return new();
            //try
            //{
            //    if (_input == null)
            //    {
            //        return new BaseResponse<RS_SearchPermissionModel>()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status400BadRequest,
            //            Message = "Dữ liệu đầu vào không hợp lệ"
            //        };
            //    }

            //    IQueryable<Permission> query = _permissionRepository.FindByCondition(x => (string.IsNullOrEmpty(_input.Name) || x.Name.Contains(_input.Name))).AsQueryable();

            //    var pagination = await _permissionRepository.OffsetPagination<Permission>(query, _input.Page, _input.PageSize);

            //    var data = _mapper.Map<List<PermissionModel>>(pagination.Items);

            //    return new BaseResponse<RS_SearchPermissionModel>()
            //    {
            //        IsSuccess = true,
            //        StatusCode = StatusCodes.Status404NotFound,
            //        Message = "Lấy dữ liệu thành công",
            //        Result = new()
            //        {
            //            Data = new()
            //            {
            //                Items = data,
            //                PageNumber = pagination.PageNumber,
            //                PerPage = pagination.PerPage,
            //                TotalItems = pagination.TotalItems,
            //                TotalPages = pagination.TotalPages,
            //            }
            //        }
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new BaseResponse<RS_SearchPermissionModel>()
            //    {
            //        IsSuccess = false,
            //        StatusCode = StatusCodes.Status200OK,
            //        Message = ex.Message,
            //    };
            //}
        }

        public async Task<BaseResponse<RS_GetPermissionModel>> Get(RQ_GetPermissionModel _input)
        {
            return new();
            //try
            //{
            //    if (_input == null)
            //    {
            //        return new BaseResponse<RS_GetPermissionModel>()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status400BadRequest,
            //            Message = "Điều kiện nhập trống"
            //        };
            //    }

            //    var productEntity = await _permissionRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

            //    var data = _mapper.Map<PermissionModel>(productEntity);

            //    return new BaseResponse<RS_GetPermissionModel>()
            //    {
            //        IsSuccess = true,
            //        StatusCode = StatusCodes.Status404NotFound,
            //        Message = "Lấy dữ liệu thành công",
            //        Result = new()
            //        {
            //            Data = data
            //        }
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new BaseResponse<RS_GetPermissionModel>()
            //    {
            //        IsSuccess = false,
            //        StatusCode = StatusCodes.Status200OK,
            //        Message = ex.Message,
            //    };
            //}
        }

        public async Task<BaseResponse> Delete(RQ_DeletePermissionModel _input)
        {
            return new();
            //try
            //{
            //    if (_input == null)
            //    {
            //        return new BaseResponse()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status400BadRequest,
            //            Message = "Điều kiện nhập trống"
            //        };
            //    }

            //    var tableEntity = await _permissionRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

            //    if (tableEntity == null)
            //    {
            //        return new BaseResponse()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status400BadRequest,
            //            Message = "Dữ liệu không tồn tại"
            //        };
            //    }

            //    var isAdmin = await _userService.CheckAdmin(_input.UserDelete, UserAdminType.SystemAdmin);

            //    if (isAdmin == null || !isAdmin.IsSuccess)
            //    {
            //        return new BaseResponse()
            //        {
            //            IsSuccess = false,
            //            StatusCode = StatusCodes.Status404NotFound,
            //            Message = "Phải là System Admin mới đủ quyền xóa",
            //        };
            //    }

            //    tableEntity.IsDeleted = true;
            //    tableEntity.DeletedDate = DateTime.Now;
            //    tableEntity.DeletedBy = isAdmin.Result.Id;

            //    await _permissionRepository.UpdateAsync(tableEntity);
            //    await _permissionRepository.SaveChangesAsync();

            //    return new BaseResponse()
            //    {
            //        IsSuccess = true,
            //        StatusCode = StatusCodes.Status404NotFound,
            //        Message = "Xóa dữ liệu thành công"
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new BaseResponse()
            //    {
            //        IsSuccess = false,
            //        StatusCode = StatusCodes.Status200OK,
            //        Message = ex.Message,
            //    };
            //}
        }
    }
}