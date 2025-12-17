using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Enums;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.MDataRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductDetailRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.ProductService
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IMDataRepository _mDataRepository;

        public ProductService(
            IOptions<AppSettings> options,
            IMapper mapper,
            IProductRepository productRepository,
            IProductDetailRepository productDetailRepository,
            IUserRepository userRepository,
            IUserService userService,
            IProductCategoryRepository productCategoryRepository,
            IMDataRepository mDataRepository
            ) : base(options, mapper)
        {
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
            _userRepository = userRepository;
            _userService = userService;
            _productCategoryRepository = productCategoryRepository;
            _mDataRepository = mDataRepository;
        }

        public async Task<BaseResponse> Search(ProductSearchRequest _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Điều kiện nhập trống");

                var query = _productRepository.FindByCondition(x =>
                    (_input.ProductCategoryId == null || x.ProductCategoryId == _input.ProductCategoryId)
                    && (string.IsNullOrEmpty(_input.FilterKeyword) || x.Name.Contains(_input.FilterKeyword))
                ).AsQueryable();

                var data = await OffsetPagination(query, _input.Page, _input.PageSize);
                var products = new PaginationModel<ProductResponseModel>()
                {
                    IsPaging = data.IsPaging,
                    PageNumber = data.PageNumber,
                    PerPage = data.PerPage,
                    TotalItems = data.TotalItems,
                    TotalPages = data.TotalPages,
                    Items = _mapper.Map<List<ProductResponseModel>>(data.Items),
                };

                var mData = await _mDataRepository.FindAll().ToListAsync();
                var productCategories = await _productCategoryRepository.FindAll().ToListAsync();

                products.Items.ForEach(product =>
                {
                    product.LoanAmount = mData.FirstOrDefault(c => c.Group == MDataEnum_Group.PRODUCT_AMOUNT && c.Key == product.LoanAmountId.ToString())?.Value ?? "";
                    product.LoanTerm = mData.FirstOrDefault(c => c.Group == MDataEnum_Group.PRODUCT_TIME && c.Key == product.LoanTermId.ToString())?.Value ?? "";
                    product.ProductCategory = productCategories.FirstOrDefault(c => c.Id == product.ProductCategoryId)?.Name ?? "";
                });

                return SuccessResponse(products);
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
                var productEntity = await _productRepository.GetByIdAsync(_id);

                if (productEntity == null)
                    return BadRequestResponse(
                        ErrorCodeConst.ERROR_REQUEST_NOT_FOUND,
                        "Sản phẩm không tồn tại"
                    );

                var mData = await _mDataRepository.FindAll().ToListAsync();
                var detailEntities = await _productDetailRepository.FindByCondition(c => c.ProductId == _id && !c.IsDeleted).ToListAsync();

                var productResponse = _mapper.Map<ProductResponseModel>(productEntity);
                productResponse.ImageUrl = $"{_appSettings.CloudflareR2.PublicBaseUrl}/{productResponse.ImageUrl}";
                productResponse.Details = _mapper.Map<List<ProductDetailResponseModel>>(detailEntities);
                productResponse.LoanAmount = mData.FirstOrDefault(c => c.Group == MDataEnum_Group.PRODUCT_AMOUNT && c.Key == productResponse.LoanAmountId.ToString())?.Value ?? "";
                productResponse.LoanTerm = mData.FirstOrDefault(c => c.Group == MDataEnum_Group.PRODUCT_TIME && c.Key == productResponse.LoanTermId.ToString())?.Value ?? "";

                return SuccessResponse(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(
                    ErrorCodeConst.ERROR_SYS_ERR,
                    ex.Message
                );
            }
        }

        public async Task<BaseResponse> Create(ProductCreateRequest _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(
                        ErrorCodeConst.ERROR_INPUT_INVALID,
                        "Điều kiện nhập trống"
                    );

                var productEntity = _mapper.Map<Product>(_input);
                productEntity.Id = Guid.NewGuid();
                productEntity.CreatedByName = _input.UserName;
                productEntity.CreatedDate = DateTime.UtcNow;

                await _productRepository.CreateAsync(productEntity);

                if (_input.Details?.Count > 0)
                {
                    var productDetails = _input.Details.Select(d =>
                    {
                        var entity = _mapper.Map<ProductDetail>(d);
                        entity.Id = Guid.NewGuid();
                        entity.ProductId = productEntity.Id;
                        entity.CreatedByName = _input.UserName;
                        entity.CreatedDate = DateTime.UtcNow;
                        return entity;
                    }).ToList();

                    await _productDetailRepository.CreateListAsync(productDetails);
                }

                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(
                    ErrorCodeConst.ERROR_SYS_ERR,
                    ex.Message
                );
            }
        }

        public async Task<BaseResponse> Update(Guid _id, ProductUpdateRequest _input)
        {
            try
            {
                if (_input == null)
                    return BadRequestResponse(
                        ErrorCodeConst.ERROR_INPUT_INVALID,
                        "Dữ liệu cập nhật không hợp lệ"
                    );

                var productEntity = await _productRepository.GetByIdAsync(_id);
                if (productEntity == null)
                    return BadRequestResponse(
                        ErrorCodeConst.ERROR_REQUEST_NOT_FOUND,
                        "Sản phẩm không tồn tại"
                    );

                // =========================
                // UPDATE PRODUCT
                // =========================
                _mapper.Map(_input, productEntity);

                if (!string.IsNullOrEmpty(productEntity.ImageUrl)
                    && productEntity.ImageUrl.Contains(_appSettings.CloudflareR2.PublicBaseUrl))
                {
                    productEntity.ImageUrl = productEntity.ImageUrl
                        .Replace(_appSettings.CloudflareR2.PublicBaseUrl, "")
                        .Replace("/", "");
                }

                productEntity.ModifiedByName = _input.UserName;
                productEntity.ModifiedDate = DateTime.Now;

                await _productRepository.UpdateAsync(productEntity);

                // =========================
                // UPDATE / INSERT / SOFT DELETE DETAILS
                // =========================
                var inputDetails = _input.Details ?? new List<ProductDetailUpdateModel>();

                // 1️⃣ Lấy toàn bộ detail hiện có của product
                // With this corrected line:
                var dbDetails = await _productDetailRepository
                    .FindByCondition(x => x.ProductId == productEntity.Id && !x.IsDeleted)
                    .ToListAsync();

                // 2️⃣ Danh sách Id từ input
                var inputDetailIds = inputDetails
                    .Where(x => x.Id.HasValue)
                    .Select(x => x.Id)
                    .ToHashSet();

                // 3️⃣ SOFT DELETE: DB có nhưng input không còn
                var deleteDetails = dbDetails
                    .Where(x => !inputDetailIds.Contains(x.Id))
                    .ToList();

                foreach (var del in deleteDetails)
                {
                    del.IsDeleted = true;
                    del.ModifiedDate = DateTime.Now;
                    del.ModifiedByName = _input.UserName;

                    await _productDetailRepository.UpdateAsync(del);
                }

                // 4️⃣ UPDATE / INSERT
                foreach (var detail in inputDetails)
                {
                    // 👉 UPDATE
                    if (detail.Id.HasValue)
                    {
                        var detailEntity = dbDetails.FirstOrDefault(x => x.Id == detail.Id.Value);
                        if (detailEntity == null)
                            continue;

                        _mapper.Map(detail, detailEntity);
                        detailEntity.ModifiedByName = _input.UserName;
                        detailEntity.ModifiedDate = DateTime.Now;

                        await _productDetailRepository.UpdateAsync(detailEntity);
                    }
                    // 👉 INSERT
                    else
                    {
                        var newDetail = _mapper.Map<ProductDetail>(detail);
                        newDetail.Id = Guid.NewGuid();
                        newDetail.ProductId = productEntity.Id;
                        newDetail.CreatedByName = _input.UserName;
                        newDetail.CreatedDate = DateTime.Now;

                        await _productDetailRepository.CreateAsync(newDetail);
                    }
                }

                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(
                    ErrorCodeConst.ERROR_SYS_ERR,
                    ex.Message
                );
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

        public async Task<BaseResponse> InitProductsApp()
        {
            try
            {
                var categoriesDb = await _productCategoryRepository
                    .FindByCondition(c => !c.Inactive)
                    .ToListAsync();

                var productsDb = await _productRepository
                    .FindByCondition(c => !c.Inactive)
                    .ToListAsync();

                var categoriesRes = new List<object>
                    {
                        new
                        {
                            Id = "all",
                            Name = "Tất cả sản phẩm"
                        }
                    };

                categoriesRes.AddRange(
                    categoriesDb.Select(c => new
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name
                    })
                );

                // 3️⃣ Map products
                var mData = await _mDataRepository.FindAll().ToListAsync();

                var productsRes = productsDb.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProductCategoryId = p.ProductCategoryId,
                    ImageUrl = $"{_appSettings.CloudflareR2.PublicBaseUrl}/{p.ImageUrl}",
                    LoanAmount = mData.FirstOrDefault(c => c.Group == MDataEnum_Group.PRODUCT_AMOUNT && c.Key == p.LoanAmountId.ToString())?.Value ?? "",
                    LoanTerm = mData.FirstOrDefault(c => c.Group == MDataEnum_Group.PRODUCT_TIME && c.Key == p.LoanTermId.ToString())?.Value ?? ""
                }).ToList();

                return SuccessResponse(new
                {
                    Categories = categoriesRes,
                    Products = productsRes
                });
            }
            catch (Exception ex)
            {
                return BadRequestResponse(
                    ErrorCodeConst.ERROR_SYS_ERR,
                    ex.Message
                );
            }
        }
    }
}