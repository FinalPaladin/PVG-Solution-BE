using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;

namespace PVG.Application.Services.ProductCategoryService
{
    public class ProductCategoryService : BaseService, IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductCategoryService(
            IOptions<AppSettings> options,
            IMapper mapper,

            IProductCategoryRepository productCategoryRepository,
            IProductRepository productRepository)
            : base(options, mapper)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
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

                var result = await _productCategoryRepository.CreateAsync(new ProductCategory()
                {
                    Id = Guid.NewGuid(),
                    Name = _input.Name,
                    Inactive = _input.Inactive,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreatedByName = _input.CreatedBy,
                    ModifiedByName = _input.CreatedBy,
                });

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
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

                IQueryable<ProductCategory> query = _productCategoryRepository.FindByCondition(
                    x => !x.Inactive
                    && (string.IsNullOrEmpty(_input.keyword) || x.Name.Contains(_input.keyword))
                ).AsQueryable();

                var pagination = await OffsetPagination<ProductCategory>(query, _input.Page, _input.PageSize);
                return SuccessResponse(pagination);
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

        public async Task<BaseResponse> Delete(Guid _id)
        {
            try
            {
                if (string.IsNullOrEmpty(_id.ToString()))
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Dữ liệu đầu vào không hợp lệ");

                var tableEntity = await _productCategoryRepository.FindByCondition(x => x.Id == _id).FirstOrDefaultAsync();
                if (tableEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Dữ liệu không tồn tại");

                var productEntity = await _productRepository.FindByCondition(x => x.IsDeleted == false && x.ProductCategoryId == tableEntity.Id).FirstOrDefaultAsync();
                if (productEntity != null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Danh mục có sản phẩm đang hiệu lực, không thể xóa");

                tableEntity.Inactive = true;
                tableEntity.ModifiedDate = DateTime.Now;

                await _productCategoryRepository.UpdateAsync(tableEntity);

                return SuccessResponse("Xóa dữ liệu thành công");
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

        public async Task<BaseResponse> Update(RQ_UpdateProductCategoryModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Dữ liệu đầu vào không hợp lệ");
                }
                var dataUpdate = await _productCategoryRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();
                if (dataUpdate == null)
                {
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Dữ liệu không tồn tại");
                }
                dataUpdate.Name = _input.Name;
                dataUpdate.Inactive = _input.Inactive;
                dataUpdate.ModifiedDate = DateTime.Now;
                await _productCategoryRepository.UpdateAsync(dataUpdate);
                await _productCategoryRepository.SaveChangesAsync();
                return SuccessResponse("Cập nhật dữ liệu thành công");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }
    }
}