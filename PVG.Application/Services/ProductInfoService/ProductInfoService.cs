using AutoMapper;
using Microsoft.AspNetCore.Http;
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

namespace PVG.Application.Services.ProductInfoService
{
    public class ProductInfoService: IProductInfoService
    {
        private readonly IProductInfoRepository _productInfoRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public ProductInfoService(IProductInfoRepository productInfoRepository,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _productInfoRepository = productInfoRepository;
            _mapper = mapper;
            _userRepository = userRepository;
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

                var dataUpdate = _productInfoRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

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

        public async Task<BaseResponse<RS_GetAllProductInfoModel>> GetAll()
        {
            try
            {
                var productsEntity = _productInfoRepository.FindAll().ToList();

                var data = _mapper.Map<List<ProductInfoModel>>(productsEntity);

                return new BaseResponse<RS_GetAllProductInfoModel>()
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
                return new BaseResponse<RS_GetAllProductInfoModel>()
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

                productEntity.IsDeleted = true;
                productEntity.DeletedDate = DateTime.Now;
                productEntity.DeletedBy = _input.DeleteUserId;

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
