using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PVG.Application.Services.NewsService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Enums;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Web.Extensions;

namespace PVG.Web.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : PVGControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly INewsService _newsService;

        public NewsController(IOptions<AppSettings> options, INewsService newsService)
        {
            _appSettings = options.Value;
            _newsService = newsService;
        }

        /// <summary>
        /// API danh sách các tin tức cho phía web
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ObjectResult> GetNewsListWeb([FromQuery] DTOSearchNews searchNews)
        {
            if (searchNews.IsPaging)
            {
                if (searchNews.Page <= 0 || searchNews.PageSize <= 0)
                {
                    return BadRequestResponse(CreateModel(null, ErrorCodeConst.ERROR_PAGGING_INPUT_INVALID, 400));
                }
            }
            searchNews.Type = NewsTypeEnum.News;
            BaseResponse response = await _newsService.GetNewsList(searchNews, false);
            return ReturnData(response);
        }

        /// <summary>
        /// API danh sách các tin tức cho phía app
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet]
        //[Route("app")]
        //public async Task<ObjectResult> GetNewsListMobile([FromQuery] DTOSearchNews searchNews)
        //{
        //    if (searchNews.IsPaging)
        //    {
        //        if (searchNews.Page <= 0 || searchNews.PageSize <= 0)
        //        {
        //            return BadRequestResponse(CreateModel(null, ErrorCodeConst.ERROR_PAGGING_INPUT_INVALID, 400));
        //        }
        //    }
        //    searchNews.ForApp = true;
        //    searchNews.Type = NewsTypeEnum.News;

        //    BaseResponse response = await _newsService.GetNewsList(searchNews);
        //    return ReturnData(response);
        //}

        /// <summary>
        /// API xem chi tiết tin tức cho phía web
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        //[Authorize(Roles = "NEWS_READ")]
        [HttpGet]
        [Route("{newsId}")]
        public async Task<ObjectResult> GetNewsWeb(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
            }

            BaseResponse response = await _newsService.GetNews(newsId);
            return ReturnData(response);
        }

        /// <summary>
        /// API xem chi tiết tin tức cho phía app
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        [Route("app/{newsId}")]
        public async Task<ObjectResult> GetNewsApp(Guid newsId, [FromQuery] string slug)
        {
            if (newsId == Guid.Empty && string.IsNullOrWhiteSpace(slug))
            {
                return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
            }

            BaseResponse response = await _newsService.GetNews(newsId, slug);
            return ReturnData(response);
        }

        /// <summary>
        /// API thêm tin tức
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        //[Authorize(Roles = "NEWS_CREATE")]
        [HttpPost]
        [Route("newsCategory/{categoryId}")]
        public async Task<ObjectResult> CreateNews(Guid categoryId, [FromBody] DTONewsRequest newsRequest)
            => ReturnData(await _newsService.CreateNews(categoryId, newsRequest));

        /// <summary>
        /// API cập nhật tin tức
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        //[Authorize(Roles = "NEWS_UPDATE")]
        [HttpPut]
        [Route("news/{newsId}/category/{categoryId}")]
        public async Task<ObjectResult> UpdateNews(Guid categoryId, Guid newsId, [FromBody] DTONewsRequest newsRequest)
        {
            if (newsId == Guid.Empty)
            {
                return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
            }

            BaseResponse response = await _newsService.UpdateNews(categoryId, newsId, newsRequest);
            return ReturnData(response);
        }

        /// <summary>
        /// API xóa tin tức
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        //[Authorize(Roles = "NEWS_DELETE")]
        [HttpDelete]
        [Route("{newsId}")]
        public async Task<ObjectResult> DeleteNews(Guid newsId)
        {
            if (newsId == Guid.Empty)
            {
                return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
            }

            BaseResponse response = await _newsService.DeleteNews(newsId);
            return ReturnData(response);
        }

        [HttpGet]
        [Route("app/slug/{slug}")]
        public async Task<ObjectResult> GetBySlug(string slug)
            => ReturnData(await _newsService.GetNewsBySlug(slug));

        [HttpGet("app/share/news/{slug}")]
        public async Task<IActionResult> ShareNews(string slug)
        {
            var result = await _newsService.GetNewsBySlug(slug);
            if (result == null)
                return NotFound();

            var news = result.Result as NewsResponseModel;
            var html = OgHtmlBuilder.BuildNewsHtml(
                title: news?.Title ?? "",
                description: news?.Description ?? "",
                image: news?.Thumbnail ?? "",
                url: $"{_appSettings.Root.PublicDomain}/tin-tuc/{news?.CategorySlug ?? ""}/{slug}"
            );

            return Content(html, "text/html; charset=utf-8");
        }

        [HttpPost]
        [Route("approve")]
        public async Task<ObjectResult> ApproveNews([FromBody] ApproveNewsRequestDto dtoApproveNews)
            => ReturnData(await _newsService.ApproveNews(dtoApproveNews));

        [HttpPost]
        [Route("unapprove")]
        public async Task<ObjectResult> UnApproveNews([FromBody] ApproveNewsRequestDto dtoApproveNews)
            => ReturnData(await _newsService.UnApproveNews(dtoApproveNews));

        [HttpGet]
        [Route("app")]
        public async Task<ObjectResult> GetAllNewForWeb()
            => ReturnData(await _newsService.GetAllForWeb());
    }
}