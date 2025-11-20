using AutoMapper;
using Microsoft.AspNetCore.Http;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
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
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _productCategoryRepository = productCategoryRepository;
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

                var dataUpdate = _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                var userEntity = _userRepository.FindByCondition(x => x.Id == _input.CreateUserId).FirstOrDefault();

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

        public async Task<BaseResponse<RS_GetAllProductCategoryModel>> GetAll()
        {
            try
            {
                var productsEntity = _productCategoryRepository.FindAll().ToList();

                var data = _mapper.Map<List<ProductCategoryModel>>(productsEntity);

                return new BaseResponse<RS_GetAllProductCategoryModel>()
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
                return new BaseResponse<RS_GetAllProductCategoryModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<RS_GetProductCategoryModel>> Get(RQ_GetProductCategoryModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_GetProductCategoryModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Điều kiện nhập trống"
                    };
                }

                var productEntity = _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                var data = _mapper.Map<ProductCategoryModel>(productEntity);

                return new BaseResponse<RS_GetProductCategoryModel>()
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
                return new BaseResponse<RS_GetProductCategoryModel>()
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

                var productEntity = _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                if (productEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu không tồn tại"
                    };
                }

                productEntity.IsDeleted = true;
                productEntity.DeletedDate = DateTime.Now;
                productEntity.DeletedBy = _input.DeleteUserId;

                await _productCategoryRepository.UpdateAsync(productEntity);
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
