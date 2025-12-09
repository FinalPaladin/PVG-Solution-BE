using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ProductCategoryService
{
    public class ProductCategoryService: IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse> Save(RQ_SaveProductCategoryModel _input)
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

                var dataUpdate = await _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                var userEntity = await _userRepository.FindByCondition(x => x.Id == _input.CreateUserId).FirstOrDefaultAsync();

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
                    dataUpdate.Name = _input.Name;
                    dataUpdate.ModifiedBy = userEntity.Id;
                    dataUpdate.ModifiedByName = userEntity.FullName;
                    dataUpdate.ModifiedDate = DateTime.Now;
                    await _productCategoryRepository.UpdateAsync(dataUpdate);
                }
                else
                {
                    var dataCreate = new ProductCategory()
                    {
                        Id = Guid.NewGuid(),
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

                        Name = _input.Name,
                    };
                    await _productCategoryRepository.CreateAsync(dataCreate);
                }

                await _productCategoryRepository.SaveChangesAsync();

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

        public async Task<BaseResponse> Search(RQ_SearchProductCategoryModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                IQueryable<ProductCategory> query = _productCategoryRepository.FindByCondition(x => x.IsDeleted == false
                    && (string.IsNullOrEmpty(_input.Name) || x.Name.Contains(_input.Name))
                ).AsQueryable();

                var pagination = await _productCategoryRepository.OffsetPagination<ProductCategory>(query, _input.Page, _input.PageSize);

                var data = _mapper.Map<List<ProductCategoryModel>>(pagination.Items);

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lấy dữ liệu thành công",
                    Result = new
                    {
                        Items = data,
                        PageNumber = pagination.PageNumber,
                        PerPage = pagination.PerPage,
                        TotalItems = pagination.TotalItems,
                        TotalPages = pagination.TotalPages,
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> Get(RQ_GetProductCategoryModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Điều kiện nhập trống"
                    };
                }

                var productEntity = await _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                var data = _mapper.Map<ProductCategoryModel>(productEntity);

                return new()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lấy dữ liệu thành công",
                    Result = data
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> Delete(RQ_DeleteProductCategoryModel _input)
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

                var tableEntity = await _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                if (tableEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu không tồn tại"
                    };
                }

                var productEntity = await _productRepository.FindByCondition(x => x.IsDeleted == false && x.ProductCategoryId == tableEntity.Id).FirstOrDefaultAsync();

                if (productEntity != null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Có sản phẩm trực thuộc còn tồn tại"
                    };
                }

                tableEntity.IsDeleted = true;
                tableEntity.DeletedDate = DateTime.Now;
                tableEntity.DeletedBy = _input.DeleteUserId;

                await _productCategoryRepository.UpdateAsync(tableEntity);
                await _productCategoryRepository.SaveChangesAsync();

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
