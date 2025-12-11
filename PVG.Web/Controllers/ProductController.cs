using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ProductService;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : PVGControllerBase
    {
        private IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] RQ_SearchProductModel _input)
        {
            var result = await _service.Search(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Save([FromBody] RQ_GetProductModel _input)
        {
            return ReturnData(await _service.Get(_input));
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveProductModel _input)
        {
            return ReturnData(await _service.Save(_input));
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Save([FromBody] RQ_DeleteProductModel _input)
        {
            return ReturnData(await _service.Delete(_input));
        }
    }
}