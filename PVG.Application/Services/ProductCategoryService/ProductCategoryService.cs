using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.ProductCategoryRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ProductCategoryService
{
    public class ProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }


    }
}
