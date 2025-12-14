using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.NewsService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Enums;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : PVGControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// API danh sách các tin tức cho phía web
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/news/list/web")]
        public async Task<ObjectResult> GetNewsListWeb([FromQuery] DTOSearchNews searchNews)
        {
            try
            {
                if (searchNews.IsPaging)
                {
                    if (searchNews.PerPage <= 0 || searchNews.PageNumber <= 0)
                    {
                        return BadRequestResponse(CreateModel(null, ErrorCodeConst.ERROR_PAGGING_INPUT_INVALID, 400));
                    }
                }
                searchNews.Type = NewsTypeEnum.News;
                BaseResponse response = await _newsService.GetNewsList(searchNews, false);
                return ReturnData(response);
            }
            catch (Exception e)
            {
                return CatchErrorResponse(e);
            }
        }

        /// <summary>
        /// API danh sách các tin tức cho phía app
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("v1/news/list/app")]
        public async Task<ObjectResult> GetNewsListMobile([FromQuery] DTOSearchNews searchNews)
        {
            try
            {
                if (searchNews.IsPaging)
                {
                    if (searchNews.Page <= 0 || searchNews.PageSize <= 0)
                    {
                        return BadRequestResponse(CreateModel(null, Constants.ERROR_PAGING, 400));
                    }
                }
                searchNews.ForApp = true;
                //searchNews.IsPaging = true;
                //searchNews.Page = 1;
                //searchNews.PageSize = 20;
                searchNews.Type = NewsTypeEnum.News;

                BaseResponse response = await _contentService.GetNewsList(searchNews);
                return ReturnData(response);
            }
            catch (Exception e)
            {
                return CatchErrorResponse(e);
            }
        }

        /// <summary>
        /// API xem chi tiết tin tức cho phía web
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        [Authorize(Roles = "NEWS_READ")]
        [HttpGet]
        [Route("v1/news/{newsId}")]
        public async Task<ObjectResult> GetNewsWeb(Guid newsId)
        {
            try
            {
                if (newsId == Guid.Empty)
                {
                    return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
                }

                BaseResponse response = await _contentService.GetNews(newsId);
                return ReturnData(response);
            }
            catch (Exception e)
            {
                return CatchErrorResponse(e);
            }
        }

        /// <summary>
        /// API xem chi tiết tin tức cho phía app
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("v1/news")]
        public async Task<ObjectResult> GetNewsMobile([FromQuery] Guid newsId, [FromQuery] string slug)
        {
            try
            {
                if (newsId == Guid.Empty && string.IsNullOrWhiteSpace(slug))
                {
                    return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
                }

                BaseResponse response = await _contentService.GetNews(newsId, slug);
                return ReturnData(response);
            }
            catch (Exception e)
            {
                return CatchErrorResponse(e);
            }
        }

        /// <summary>
        /// API thêm tin tức
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        [Authorize(Roles = "NEWS_CREATE")]
        [HttpPost]
        [Route("v1/news/{categoryId}")]
        public async Task<ObjectResult> CreateNews(Guid categoryId, [FromBody] DTONewsRequest newsRequest)
            => ReturnData(await _contentService.CreateNews(categoryId, newsRequest));

        /// <summary>
        /// API cập nhật tin tức
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        [Authorize(Roles = "NEWS_UPDATE")]
        [HttpPut]
        [Route("v1/news/{newsId}/category/{categoryId}")]
        public async Task<ObjectResult> UpdateNews(Guid categoryId, Guid newsId, [FromBody] DTONewsRequest newsRequest)
        {
            try
            {
                if (newsId == Guid.Empty)
                {
                    return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
                }

                BaseResponse response = await _contentService.UpdateNews(categoryId, newsId, newsRequest);
                return ReturnData(response);
            }
            catch (Exception e)
            {
                return CatchErrorResponse(e);
            }
        }

        /// <summary>
        /// API xóa tin tức
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "CNT_MANAGENEWS")]
        [Authorize(Roles = "NEWS_DELETE")]
        [HttpDelete]
        [Route("v1/news/{newsId}")]
        public async Task<ObjectResult> DeleteNews(Guid newsId)
        {
            try
            {
                if (newsId == Guid.Empty)
                {
                    return BadRequestResponse(CreateModel(null, "newsId_ERR_REQUIRED", 400));
                }

                BaseResponse response = await _contentService.DeleteNews(newsId);
                return ReturnData(response);
            }
            catch (Exception e)
            {
                return CatchErrorResponse(e);
            }
        }
    }
}
