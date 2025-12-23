using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;

namespace PVG.Application.Services.NewsService
{
    public interface INewsService
    {
        Task<BaseResponse> GetNewsList(DTOSearchNews searchNews, bool isMobile = true);

        Task<BaseResponse> GetNews(Guid id, string slug = null);

        Task<BaseResponse> GetNewsBySlug(string _slug);

        Task<BaseResponse> CreateNews(Guid categoryId, DTONewsRequest newsRequest);

        Task<BaseResponse> CreateNews(News news, Guid? categoryId, DTOFile thumbnail = null, List<DTOFile> attachments = null, bool isNotify = false);

        Task<BaseResponse> UpdateNews(Guid categoryId, Guid newsId, DTONewsRequest newsRequest);

        Task<BaseResponse> UpdateNews(News news, Guid? categoryId, DTOFile thumbnail = null, List<DTOFile> attachments = null, bool isNotify = false);

        //Task<BaseResponse> UpdateNewsOrder(DTONewsOrderRequest newsOrderRequest);

        Task<BaseResponse> DeleteNews(Guid id);

        Task<BaseResponse> GetAllForWeb();

        Task<BaseResponse> ApproveNews(ApproveNewsRequestDto _payload);
        Task<BaseResponse> UnApproveNews(ApproveNewsRequestDto _payload);
    }
}