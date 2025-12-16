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
        public async Task<IActionResult> Search([FromQuery] RQ_SearchProductModel _input)
           => ReturnData(await _service.Search(_input));

        [HttpGet]
        public async Task<IActionResult> Save([FromQuery] RQ_GetProductModel _input)
            => ReturnData(await _service.Get(_input));

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] RQ_SaveProductModel _input)
            => ReturnData(await _service.Save(_input));



        [HttpDelete]
        public async Task<IActionResult> Save([FromBody] RQ_DeleteProductModel _input)
        {
            return ReturnData(await _service.Delete(_input));
        }
    }
}