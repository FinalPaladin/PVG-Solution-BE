using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ProductInfoRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.ProductInfoService
{
    public class ProductInfoService: IProductInfoService
    {
        private readonly IProductInfoRepository _productInfoRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public ProductInfoService(IProductInfoRepository productInfoRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IUserService userService)
        {
            _productInfoRepository = productInfoRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<BaseResponse> Save(RQ_SaveProductInfoModel _input)
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

                var dataUpdate = await _productInfoRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                var userEntity = await _userRepository.FindByCondition(x => x.UserName == _input.CreateUser && x.Actived).FirstOrDefaultAsync();

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
                    dataUpdate.Content = _input.Content;
                    dataUpdate.Type = _input.Type;
                    dataUpdate.Description = _input.Description;
                    dataUpdate.ProductId = _input.ProductId;
                    dataUpdate.ModifiedBy = userEntity.Id;
                    dataUpdate.ModifiedByName = userEntity.FullName;
                    dataUpdate.ModifiedDate = DateTime.Now;
                    await _productInfoRepository.UpdateAsync(dataUpdate);
                }
                else
                {
                    var dataCreate = new ProductInfo()
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

                        Content = _input.Content,
                        Type = _input.Type,
                        Description = _input.Description,
                        ProductId = _input.ProductId,
                    };
                    await _productInfoRepository.CreateAsync(dataCreate);
                }

                await _productInfoRepository.SaveChangesAsync();

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

        public async Task<BaseResponse<RS_SearchProductInfoModel>> Search(RQ_SearchProductInfoModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_SearchProductInfoModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                IQueryable<ProductInfo> query = _productInfoRepository.FindByCondition(x => x.IsDeleted == false
                    && (
                        ((_input.ProductId == null) && ((string.IsNullOrEmpty(_input.Type) || x.Type == x.Type)
                        && (string.IsNullOrEmpty(_input.Description) || x.Description.Contains(_input.Description))
                        && (string.IsNullOrEmpty(_input.Content) || x.Content.Contains(_input.Content))))
                        || (_input.ProductId == x.ProductId)
                    )
                ).AsQueryable();

                var pagination = await _productInfoRepository.OffsetPagination<ProductInfo>(query, _input.Page, _input.PageSize);

                var data = _mapper.Map<List<ProductInfoModel>>(pagination.Items);

                return new BaseResponse<RS_SearchProductInfoModel>()
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
                return new BaseResponse<RS_SearchProductInfoModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<RS_GetProductInfoModel>> Get(RQ_GetProductInfoModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_GetProductInfoModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Điều kiện nhập trống"
                    };
                }

                var productEntity = _productInfoRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                var data = _mapper.Map<ProductInfoModel>(productEntity);

                return new BaseResponse<RS_GetProductInfoModel>()
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
                return new BaseResponse<RS_GetProductInfoModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> Delete(RQ_DeleteProductInfoModel _input)
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

                var productEntity = _productInfoRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                if (productEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Sản phẩm không tồn tại"
                    };
                }

                var AD = UserAdminType.SystemAdmin;

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

                productEntity.IsDeleted = true;
                productEntity.DeletedDate = DateTime.Now;
                productEntity.DeletedBy = isAdmin.Result.Id;

                await _productInfoRepository.UpdateAsync(productEntity);
                await _productInfoRepository.SaveChangesAsync();

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
