using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ProductCategoryService;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/product/category")]
    [ApiController]
    public class ProductCategoryController : PVGControllerBase
    {
        private IProductCategoryService _service;

        public ProductCategoryController(IProductCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] RQ_SearchProductCategoryModel _input)
            => ReturnData(await _service.Search(_input));

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RQ_GetProductCategoryModel _input)
            => ReturnData(await _service.Get(_input));

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] RQ_SaveProductCategoryModel _input)
            => ReturnData(await _service.Save(_input));

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid _id)
            => ReturnData(await _service.Delete(_id));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RQ_UpdateProductCategoryModel _input)
            => ReturnData(await _service.Update(_input));
    }
}