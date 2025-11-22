using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.ProductCategoryService;
using PVG.Application.Services.ProductService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/productcategory")]
    [ApiController]
    public class ProductCategoryController : PVGControllerBase
    {
        IProductCategoryService _service;
        public ProductCategoryController(IProductCategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] RQ_SearchProductCategoryModel _input)
        {
            var result = await _service.Search(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Save([FromBody] RQ_GetProductCategoryModel _input)
        {
            return ReturnData(await _service.Get(_input));
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveProductCategoryModel _input)
        {
            return ReturnData(await _service.Save(_input));
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Save([FromBody] RQ_DeleteProductCategoryModel _input)
        {
            return ReturnData(await _service.Delete(_input));
        }
    }
}
