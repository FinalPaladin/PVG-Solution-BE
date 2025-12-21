using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.NewsCategoryService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/news/category")]
    [ApiController]
    public class NewsCategoryController : PVGControllerBase
    {
        private readonly INewsCategoryService _newsCategoryService;

        public NewsCategoryController(INewsCategoryService newsCategoryService)
        { _newsCategoryService = newsCategoryService; }

        #region APIs

        /// <summary>
        /// Tìm kiếm danh mục bài đăng
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ObjectResult> GetCategory([FromQuery] NewsCategorySearchModel input)
            => ReturnData(await _newsCategoryService.GetCategory(input, false));

        /// <summary>
        /// Lấy danh mục bài đăng theo Id
        /// </summary>
        /// <param name="id">Id danh mục bài đăng</param>
        /// <returns></returns>
        //[Authorize(Roles = "NEWS_CATEGORY_READ")]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ObjectResult> GetCategoryById(Guid id)
            => ReturnData(await _newsCategoryService.GetCategoryById(id));

        /// <summary>
        /// Tạo danh mục bài đăng
        /// </summary>
        /// <param name="category">Thông tin danh mục bài đăng</param>
        /// <returns></returns>
        //[Authorize(Roles = "NEWS_CATEGORY_CREATE")]
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ObjectResult> CreateCategory([FromBody] NewsCategoryModel category)
            => ReturnData(await _newsCategoryService.CreateCategory(category, category.UserName));

        /// <summary>
        /// Cập nhật thông tin danh mục bài đăng
        /// </summary>
        /// <param name="id">Id danh mục bài đăng</param>
        /// <param name="category">Thông tin thay đổi</param>
        /// <returns></returns>
        //[Authorize(Roles = "NEWS_CATEGORY_UPDATE")]
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ObjectResult> UpdateCategory(Guid id, [FromBody] NewsCategoryModel category)
            => ReturnData(await _newsCategoryService.UpdateCategory(id, category, category.UserName));

        /// <summary>
        /// Xóa danh mục bài đăng
        /// </summary>
        /// <param name="id">Id danh mục</param>
        /// <returns></returns>
        //[Authorize(Roles = "NEWS_CATEGORY_DELETE")]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ObjectResult> DeleteCategory(Guid id)
            => ReturnData(await _newsCategoryService.DeleteCategory(id));

        /// <summary>
        /// Get danh mục bài đăng cho app
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns></returns>
        [HttpGet]
        [Route("app")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ObjectResult> GetCategoriesMobile([FromQuery] NewsCategorySearchModel input)
            => ReturnData(await _newsCategoryService.GetCategory(input));


        [HttpGet]
        [Route("all")]
        public async Task<ObjectResult> GetAll()
            => ReturnData(await _newsCategoryService.GetAll());

        #endregion APIs
    }
}