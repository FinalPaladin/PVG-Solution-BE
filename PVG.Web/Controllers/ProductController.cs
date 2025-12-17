using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ProductService;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : PVGControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // =========================
        // SEARCH (paging + filter)
        // =========================
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProductSearchRequest input)
            => ReturnData(await _service.Search(input));

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
            => ReturnData(await _service.GetById(id));

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest input)
            => ReturnData(await _service.Create(input));

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] ProductUpdateRequest input)
            => ReturnData(await _service.Update(id, input));

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            [FromQuery] string userName)
            => ReturnData(await _service.Delete(id, userName));

        [HttpGet]
        [Route("app/init")]
        public async Task<IActionResult> InitProductPage()
            => ReturnData(await _service.InitProductsApp());

        [HttpGet("app/{id:guid}")]
        public async Task<IActionResult> GetByIdPage(Guid id)
            => ReturnData(await _service.GetById(id));
    }
}