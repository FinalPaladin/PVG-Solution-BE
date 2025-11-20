using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.ProductService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : PVGControllerBase
    {
        IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAll();
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Save([FromBody] RQ_GetProductModel _input)
        {
            return ReturnData(await _productService.Get(_input));
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveProductModel _input)
        {
            return ReturnData(await _productService.Save(_input));
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Save([FromBody]RQ_DeleteProductModel _input)
        {
            return ReturnData(await _productService.Delete(_input));
        }
    }
}
