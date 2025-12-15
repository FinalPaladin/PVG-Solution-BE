using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.ProductService
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public ProductService(IOptions<AppSettings> options,
            IMapper mapper, IProductRepository productRepository,
            IUserRepository userRepository,
            IUserService userService) : base(options, mapper)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<BaseResponse> Save(RQ_SaveProductModel _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var id = Guid.NewGuid();

                var dataUpdate = await _productRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefaultAsync();

                var userEntity = await _userRepository.FindByCondition(x => x.UserName == _input.CreateUser && x.Actived).FirstOrDefaultAsync();

                if (userEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Người dùng không tồn tại.");

                if (dataUpdate != null)
                {
                    dataUpdate.Name = _input.Name;
                    dataUpdate.Image = _input.Image;
                    dataUpdate.Description = _input.Description;
                    dataUpdate.ProductCategoryId = _input.ProductCategoryId;
                    dataUpdate.ModifiedBy = userEntity.Id;
                    dataUpdate.ModifiedByName = userEntity.FullName;
                    dataUpdate.ModifiedDate = DateTime.Now;
                    await _productRepository.UpdateAsync(dataUpdate);
                }
                else
                {
                    var dataCreate = new Product()
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
                        Description = _input.Description,
                        Image = _input.Image,
                        ProductCategoryId = _input.ProductCategoryId,
                    };
                    await _productRepository.CreateAsync(dataCreate);
                }

                await _productRepository.SaveChangesAsync();

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

        public async Task<BaseResponse> Search(RQ_SearchProductModel _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var query = _productRepository.FindByCondition(x => x.IsDeleted == false
                    && (
                        ((_input.ProductCategoryId == null) && ((string.IsNullOrEmpty(_input.Name) || x.Name.Contains(_input.Name))
                        && (string.IsNullOrEmpty(_input.Description) || x.Description.Contains(_input.Description))))
                        || (_input.ProductCategoryId == x.ProductCategoryId)
                    )
                ).AsQueryable();

                var pagination = await _productRepository.OffsetPagination<Product>(query, _input.Page, _input.PageSize);

                var data = _mapper.Map<List<ProductModel>>(pagination.Items);

                return SuccessResponse(new
                {
                    Items = data,
                    PageNumber = pagination.PageNumber,
                    PerPage = pagination.PerPage,
                    TotalItems = pagination.TotalItems,
                    TotalPages = pagination.TotalPages,
                });
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }

        public async Task<BaseResponse> Create(RQ_SaveProductModel _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");
                
                var productEntity = _mapper.Map<Product>(_input);
                productEntity.Id = Guid.NewGuid();
                productEntity.Name = _input.Name;

                productEntity.Description = _input.Description;
                productEntity.Image = _input.Image;
                await _productRepository.CreateAsync(productEntity);

                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }

        public async Task<BaseResponse> Get(RQ_GetProductModel _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var productEntity = _productRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                var data = _mapper.Map<ProductModel>(productEntity);

                return SuccessResponse(data);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }

        public async Task<BaseResponse> Delete(RQ_DeleteProductModel _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var productEntity = _productRepository.FindByCondition(x => x.Id == _input.Id).FirstOrDefault();

                if (productEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Sản phẩm không tồn tại");

                var AD = UserAdminType.SystemAdmin;

                var isAdmin = await _userService.CheckAdmin(_input.UserDelete, AD);

                if (isAdmin == null || !isAdmin.IsSuccess)
                {
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, string.Format("Phải là {0} mới đủ quyền xóa", nameof(AD)));
                }

                productEntity.IsDeleted = true;
                productEntity.DeletedDate = DateTime.Now;
                productEntity.DeletedBy = isAdmin.Result.Id;

                await _productRepository.UpdateAsync(productEntity);
                await _productRepository.SaveChangesAsync();

                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }
    }
}