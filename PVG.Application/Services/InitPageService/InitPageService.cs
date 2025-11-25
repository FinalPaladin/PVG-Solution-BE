using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVG.Application.Services.InitPageService
{
    public class InitPageService: IInitPageService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;

        public InitPageService(IProductRepository productRepository,
            IProductCategoryRepository productCategoryRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<BaseResponse<ProductInitPageModel>> Product()
        {
            try
            {
                var productCategoriesEntity = await _productCategoryRepository.FindByCondition(x => !x.IsDeleted).ToListAsync();

                if(productCategoriesEntity == null || productCategoriesEntity.Count == 0)
                {
                    return new BaseResponse<ProductInitPageModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu chưa được định nghĩa"
                    };
                }

                var productcategories = _mapper.Map<List<ProductCategoryModel>>(productCategoriesEntity);

                var productsEntity = await _productRepository.FindByCondition(x => !x.IsDeleted).ToListAsync();

                if (productCategoriesEntity != null && productCategoriesEntity.Count > 0)
                {
                    var products = _mapper.Map<List<ProductModel>>(productsEntity);
                    foreach (var pce in productcategories)
                    {
                        var productsFilter = products.Where(x => x.ProductCategoryId == pce.Id).ToList();

                        if (productsFilter != null && productsFilter.Count > 0)
                            pce.Products = productsFilter;
                        else
                            pce.Products = new();
                    }
                }

                return new BaseResponse<ProductInitPageModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Lấy dữ liệu thành công",
                    Result = new()
                    {
                        Data = productcategories
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductInitPageModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> System()
        {
            try
            {
                
                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

    }
}
