using PVG.Infrastucture.Repositories.ProductInfoRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ProductInfoService
{
    public class ProductInfoService
    {
        private readonly IProductInfoRepository _productInfoRepository;

        public ProductInfoService(IProductInfoRepository productInfoRepository)
        {
            _productInfoRepository = productInfoRepository;
        }
    }
}
