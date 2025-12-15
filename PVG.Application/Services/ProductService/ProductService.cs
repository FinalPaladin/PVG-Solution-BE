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

        public async Task<BaseResponse> Search(ProductSearchRequest _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var query = _productRepository.FindByCondition(x => 
                    !x.Inactive
                    && (_input.ProductCategoryId == null || x.ProductCategoryId == _input.ProductCategoryId)
                    && (string.IsNullOrEmpty(_input.FilterKeyword) || x.Name.Contains(_input.FilterKeyword))
                ).AsQueryable();

                var data = await OffsetPagination(query, _input.PageNumber, _input.PerPage);

                return SuccessResponse(data);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }

        public async Task<BaseResponse> Create(ProductCreateRequest _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var productEntity = _mapper.Map<Product>(_input);
                productEntity.Id = Guid.NewGuid();
                productEntity.CreatedByName = _input.UserName;
                await _productRepository.CreateAsync(productEntity);

                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }

        public async Task<BaseResponse> Update(Guid _id, ProductUpdateRequest _input)
        {
            try
            {
                var productEntity = await _productRepository.GetByIdAsync(_id);


                if (productEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Sản phẩm không tồn tại");


                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }

        public async Task<BaseResponse> GetById(Guid _id)
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

        public async Task<BaseResponse> Delete(Guid _id, string _userName)
        {
            try
            {
                if (_userName == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var productEntity = await _productRepository.GetByIdAsync(_id);

                if (productEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Sản phẩm không tồn tại");

                var AD = UserAdminType.SystemAdmin;

                var isAdmin = await _userService.CheckAdmin(_userName, AD);

                if (isAdmin == null || !isAdmin.IsSuccess)
                {
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, string.Format("Phải là {0} mới đủ quyền xóa", nameof(AD)));
                }

                productEntity.Inactive = true;
                productEntity.ModifiedDate = DateTime.Now;
                productEntity.ModifiedBy = isAdmin.Result.Id;

                await _productRepository.UpdateAsync(productEntity);
                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, ex.Message);
            }
        }
    }
}