using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using PVG.Application.Services.ViewLogService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Enums;
using PVG.Domain.Extensions;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Domain.Utilities;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.NewsCategoryMappingRepository;
using PVG.Infrastucture.Repositories.NewsCategoryRepository;
using PVG.Infrastucture.Repositories.NewsRepository;
using static PVG.Domain.Enums.ViewLogEnum;

namespace PVG.Application.Services.NewsService
{
    public class NewsService : BaseService, INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly INewsCategoryRepository _categoryRepository;
        private readonly INewsCategoryMappingRepository _newsCategoryMappingRepository;
        private readonly IViewLogService _viewLogService;
        public NewsService(
            IOptions<AppSettings> settings, 
            IMapper mapper,
            INewsRepository newsRepository,
            INewsCategoryRepository categoryRepository,
            INewsCategoryMappingRepository newsCategoryMappingRepository,
            IViewLogService viewLogService
            ) : base(settings, mapper)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _newsRepository = newsRepository;
            _viewLogService = viewLogService;
            _newsCategoryMappingRepository = newsCategoryMappingRepository;
        }

        /// <summary>
        /// Xem danh sách bài đăng
        /// </summary>
        /// <param name="searchNews"></param>
        /// <returns></returns>
        public async Task<BaseResponse> GetNewsList(DTOSearchNews searchNews, bool isApp = true)
        {
            try
            {
                //var loginContactId = _claimsPrincipalExtension.GetUserId();
                //if (loginContactId == Guid.Empty)
                //    return BadRequestResponse(ErrorCodeConst.ERROR_USER_NOT_FOUND, "login_ERR_NOT_FOUND");

                searchNews.Search = searchNews.Search?.ToLower();

                var listNewsCategory = new List<NewsCategoryMapping>();
                if (searchNews.CategoryId != null)
                    listNewsCategory = await _newsCategoryMappingRepository
                            .FindByCondition(m => !m.IsDeleted && m.CategoryId == searchNews.CategoryId)
                            .ToListAsync();

                var today = DateTime.Now;

                //List<DTONewsResponse>
                var newsList = await _newsRepository.FindByCondition(m =>
                    !m.IsDeleted && (!isApp || m.PublishDate.Date <= today.Date)
                    && (!isApp || m.Active)
                    && (searchNews.CategoryId == null || listNewsCategory.Select(c => c.NewsId).Contains(m.Id))
                    && (searchNews.HasDisplayOrder == null
                        || (searchNews.HasDisplayOrder == true && m.DisplayOrder != null)
                        || (searchNews.HasDisplayOrder == false && m.DisplayOrder == null))
                    && (searchNews.Active == null || m.Active == searchNews.Active)
                    && (searchNews.Type == null || m.Type == searchNews.Type)
                    && (searchNews.Search == null || m.Title.ToLower().Contains(searchNews.Search))
                    && (!isApp || (m.ExpireDate == null || (m.ExpireDate != null && today <= m.ExpireDate)))
                    && (!isApp || !m.NeedApproved || (m.NeedApproved && m.IsApproved == true))
                    && (searchNews.CreatedDateFrom == null || m.CreatedDate >= searchNews.CreatedDateFrom)
                    && (searchNews.CreatedDateTo == null || m.CreatedDate <= searchNews.CreatedDateTo)
                    && (isApp ||
                        ((searchNews.PublishFrom == null || searchNews.PublishFrom <= m.PublishDate)
                        && (searchNews.PublishTo == null || m.PublishDate <= searchNews.PublishTo)))
                    )
                    .AsNoTracking()
                    .ToListAsync();

                switch (searchNews.SortIndex)
                {
                    case NewsColumn.ModifiedDate:
                        newsList = searchNews.SortType == SortDirection.Desc
                            ? newsList.OrderByDescending(c => c.ModifiedDate).ToList()
                            : newsList.OrderBy(c => c.ModifiedDate).ToList();
                        break;

                    case NewsColumn.Status:
                        newsList = searchNews.SortType == SortDirection.Desc
                            ? newsList.OrderByDescending(c => c.Active).ToList()
                            : newsList.OrderBy(c => c.Active).ToList();
                        break;

                    case NewsColumn.DisplayOrder:
                        newsList = searchNews.SortType == SortDirection.Desc
                            ? newsList.OrderByDescending(c => c.DisplayOrder).ToList()
                            : newsList.OrderBy(c => c.DisplayOrder).ToList();
                        break;

                    default:
                        newsList = newsList.OrderByDescending(c => c.CreatedDate).ThenBy(c => c.DisplayOrder).ToList();
                        break;
                }

                var response = _mapper.Map<List<NewsResponseModel>>(newsList);
                var listNewsIds = response.Select(c => c.Id).ToList();
                if (listNewsIds?.Count > 0)
                {
                    var listNewsCategoryUpdate = await _newsCategoryMappingRepository.FindByCondition(c => listNewsIds.Contains((Guid)c.NewsId) && !c.IsDeleted).AsNoTracking().ToListAsync();
                    if (listNewsCategoryUpdate?.Count > 0)
                    {
                        var listCategory = await _categoryRepository
                            .FindByCondition(c => listNewsCategoryUpdate.Select(m => m.CategoryId).Contains(c.Id))
                            .AsNoTracking()
                            .ToListAsync();
                        if (listCategory?.Count > 0)
                        {
                            foreach (var item in listNewsCategoryUpdate)
                            {
                                response.ForEach(c =>
                                {
                                    if (c.Id == item.NewsId)
                                    {
                                        c.CategoryId = item.CategoryId;
                                        c.CategoryName = listCategory.FirstOrDefault(m => m.Id == item.CategoryId)?.Name!;
                                    }
                                });
                            }
                        }                        
                    }
                }

                if (searchNews.IsPaging)
                    return SuccessResponse(Paging(response, searchNews.Page, searchNews.PageSize), "success");
                else
                    return SuccessResponse(response, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        /// <summary>
        /// Xem chi tiết bài đăng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaseResponse> GetNews(Guid id, string slug = null)
        {
            try
            {
                var news = await _newsRepository.FindByCondition(m => m.Id == id && (slug == null || m.Slug.Contains(slug))).FirstOrDefaultAsync();
                if (news == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Không tìm thấy tin tức phù hợp");

                await _viewLogService.Save(new()
                {
                    DetailId = id,
                    Screen = ScreenView.News
                });
                var res = _mapper.Map<NewsResponseModel>(news);
                var newsCategoryUpdate = await _newsCategoryMappingRepository.FindByCondition(c => c.NewsId == res.Id).FirstOrDefaultAsync();
                if (newsCategoryUpdate != null)
                {
                    var category = await _categoryRepository.FindByCondition(c => c.Id == newsCategoryUpdate.CategoryId).FirstOrDefaultAsync();
                    res.CategoryId = newsCategoryUpdate.CategoryId;
                    res.CategoryName = category.Name;
                }

                return SuccessResponse(res);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetNewsBySlug(string _slug)
        {
            try
            {
                var news = await _newsRepository.FindByCondition(m => m.Slug == _slug).FirstOrDefaultAsync();
                if (news == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Không tìm thấy tin tức phù hợp");

                var res = _mapper.Map<NewsResponseModel>(news);
                var newsCategoryUpdate = await _newsCategoryMappingRepository.FindByCondition(c => c.NewsId == res.Id).FirstOrDefaultAsync();
                if (newsCategoryUpdate != null)
                {
                    var category = await _categoryRepository.FindByCondition(c => c.Id == newsCategoryUpdate.CategoryId).FirstOrDefaultAsync();
                    res.CategoryId = newsCategoryUpdate.CategoryId;
                    res.CategoryName = category.Name;
                }

                return SuccessResponse(res);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        /// <summary>
        /// Thêm tin tức
        /// </summary>
        /// <param name="newsRequest"></param>
        /// <returns></returns>
        public async Task<BaseResponse> CreateNews(Guid categoryId, DTONewsRequest newsRequest)
        {
            var today = DateTime.Now;
            var news = new News()
            {
                Id = Guid.NewGuid(),
                Title = newsRequest.Title,
                Content = newsRequest.Content,
                Type = NewsTypeEnum.News,
                Description = newsRequest.Description,
                NeedApproved = true,
                PublishDate = newsRequest.PublishDate ?? today,
                ExpireDate = newsRequest.ExpireDate,
                ImageLink = newsRequest.ThumbnailFile.Path,
                ImageName = newsRequest.ThumbnailFile.FileName,
                Thumbnail = newsRequest.ThumbnailFile.Path,
                ThumbnailName = newsRequest.ThumbnailFile.FileName,
                Active = newsRequest.Active,
                DisplayOrder = newsRequest.DisplayOrder,
                //CreatedBy = _claimsPrincipalExtension.GetUserId(),
                CreatedDate = today,
            };

            return await CreateNews(news, categoryId, newsRequest.ThumbnailFile);
        }

        /// <summary>
        /// Thêm tin tức
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        public async Task<BaseResponse> CreateNews(News news, Guid? categoryId, DTOFile thumbnail = null, List<DTOFile> attachments = null, bool isNotify = false)
        {
            try
            {
                //var loginContactId = _claimsPrincipalExtension.GetUserId();
                //if (loginContactId == Guid.Empty)
                //    return BadRequestResponse(ErrorCodeConst.ERROR_USER_NOT_FOUND, "Không tìm thấy user");

                var category = await _categoryRepository.FindByCondition(x => x.Id == categoryId && x.Type == news.Type).FirstOrDefaultAsync();
                if (category == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Mã nhóm nhóm tin tức không hợp lệ");

                var files = new List<DTOFile>();

                //tạo code
                var latestNews = await _newsRepository
                    .FindByCondition(x => x.Type == news.Type)
                    .OrderByDescending(x => x.Code)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                news.Code = latestNews != null ? latestNews.Code + 1 : 1;
                news.Slug = CreateSlug(news.Title, news.Code.ToString() ?? Guid.NewGuid().ToString());

                //cập nhật đường dẫn thumbnail
                //if (thumbnail != null)
                //{
                //    var thumbnailMove = await fileService.MoveFile(thumbnail);
                //    files.Add(thumbnailMove);
                //    news.Thumbnail = thumbnailMove.Path;
                //}

                ////cập nhật đường dẫn các file đính kèm
                //if (attachments != null && attachments.Count > 0)
                //{
                //    attachments = await fileService.MoveFile(attachments);
                //    files.AddRange(attachments);
                //}

                //if (files.Count > 0)
                //{
                //    var fileBusiness = new FileBusiness();
                //    var saveFiles = new List<FileEntity>();

                //    foreach (var item in files)
                //    {
                //        var file = fileBusiness.CreateFileModel(item.FileName, item.Path, news.Id);
                //        saveFiles.Add(file);
                //    }

                //    await fileBusiness.CreateFile(saveFiles, loginContactId);
                //}

                using (var trans = await _newsRepository.BeginTransactionAsync())
                {
                    try
                    {
                        await _newsRepository.CreateAsync(news);

                        //tạo category
                        if (categoryId != null)
                        {
                            var newsCategory = new NewsCategoryMapping()
                            {
                                Id = Guid.NewGuid(),
                                CategoryId = categoryId.Value,
                                NewsId = news.Id,
                                //CreatedBy = loginContactId,
                                CreatedDate = DateTime.Now,
                            };
                            await _newsCategoryMappingRepository.CreateAsync(newsCategory);
                        }

                        await trans.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await _newsRepository.RollbackTransactionAsync();
                        Logger.Error(ex);
                        return CatchErrorResponse(ex);
                    }
                }

                //if (news.Status == 1 && news.Type == (int)NewsTypeEnum.News)
                //if (isNotify)
                //{
                //    var callEventBus = new CallEventBus(_logger);
                //    var objNoti = new DTOSendNotiNews()
                //    {
                //        Title = news.Title,
                //        Category = category?.Name,
                //        StockCode = news.StockCode,
                //        CategoryId = category?.Id,
                //        NewsId = news?.Id,
                //    };
                //    await callEventBus.SendNotiNews(objNoti, true);
                //}

                return SuccessResponse(news.Id, "success");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Update tin tức
        /// </summary>
        /// <param name="newsRequest"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> UpdateNews(Guid categoryId, Guid newsId, DTONewsRequest newsRequest)
        {
            //var loginContactId = _claimsPrincipalExtension.GetUserId();
            //if (loginContactId == Guid.Empty)
            //    return BadRequestResponse(ErrorCodeConst.not, "login_ERR_NOT_FOUND");

            var newsDB = await _newsRepository
                .FindByCondition(x => x.Id == newsId && !x.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (newsDB == null)
                return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "newsId_ERR_NOT_FOUND");

            newsDB = await _newsRepository.UpdateValueAsync(newsDB, newsRequest);
            //newsDB.ModifiedBy = loginContactId;
            newsDB.ModifiedDate = DateTime.Now;

            return await UpdateNews(newsDB, categoryId, newsRequest.ThumbnailFile);
        }

        /// <summary>
        /// Cập nhật bài đăng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaseResponse> UpdateNews(News news, Guid? categoryId, DTOFile thumbnail = null, List<DTOFile> attachments = null, bool isNotify = false)
        {
            try
            {
                //var loginContactId = _claimsPrincipalExtension.GetUserId();
                //if (loginContactId == Guid.Empty)
                //    return BadRequestResponse(ErrorCodeConst.ERROR_USER_NOT_FOUND, "login_ERR_NOT_FOUND");

                //FileEntity thumbnailOld = null;
                //List<FileEntity> attachmentsOld = new List<FileEntity>();
                //var filesAdd = new List<DTOFile>();
                //var filesDelete = new List<FileEntity>();
                //var fileService = new FileHandle(
                //                        EventBusPublisher.CallEventBusAsync,
                //                        currentServiceName: ConstEventBus.CURRENT_SERVICE,
                //                        currnetExchangeName: ConstEventBus.CURRENT_EXCHANGE,
                //                        fileServiceName: ConstEventBus.SERVICE_FILE,
                //                        fileExchangeName: ConstEventBus.EXCHANGE_FILE
                //                    );

                ////lấy các file cũ của tin đăng
                //var fileBusiness = new FileBusiness();
                //var files = await fileBusiness.GetFileList(news.Id);
                //if (files != null && files.Count > 0)
                //{
                //    thumbnailOld = files.FirstOrDefault(x => x.Path.Equals(news.Thumbnail));
                //    if (thumbnailOld != null) files.Remove(thumbnailOld);
                //    attachmentsOld = files;
                //}

                ////cập nhật đường dẫn thumbnail
                //if (thumbnail != null)
                //{
                //    //nếu đường dẫn thumbnail bị đổi thì đi cập nhật lại
                //    if (!thumbnail.Path.Equals(news.Thumbnail))
                //    {
                //        //xóa file ở đường dẫn cũ
                //        if (thumbnailOld != null)
                //        {
                //            await fileService.DeleteFile(thumbnailOld);
                //            filesDelete.Add(thumbnailOld);
                //        }

                //        //thêm file mới
                //        var thumbnailNew = await fileService.MoveFile(thumbnail);
                //        filesAdd.Add(thumbnailNew);
                //        news.Thumbnail = thumbnailNew.Path;
                //    }
                //}

                ////cập nhật đường dẫn các file đính kèm
                //if (attachments != null && attachments.Count > 0)
                //{
                //    var listDeleteFile = attachmentsOld.Where(e => !attachments.Any(f => f.Path == e.Path)).ToList();
                //    var listMoveFile = attachments.Where(e => !attachmentsOld.Any(f => f.Path == e.Path)).ToList();
                //    await fileService.DeleteFile(listDeleteFile);
                //    listMoveFile = await fileService.MoveFile(listMoveFile);

                //    filesDelete.AddRange(listDeleteFile);
                //    filesAdd.AddRange(listMoveFile);
                //}
                //else
                //{
                //    filesDelete.AddRange(attachmentsOld);
                //}

                ////thêm file vào bảng CR_File
                //if (filesAdd.Count > 0)
                //{
                //    var saveFiles = new List<FileEntity>();

                //    foreach (var item in filesAdd)
                //    {
                //        var file = fileBusiness.CreateFileModel(item.FileName, item.Path, news.Id);
                //        saveFiles.Add(file);
                //    }

                //    await fileBusiness.CreateFile(saveFiles, loginContactId);
                //}

                ////xóa file vào bảng CR_File
                //if (filesDelete.Count > 0)
                //{
                //    await fileBusiness.DeleteFile(filesDelete, loginContactId);
                //}

                using (var trans = await _newsRepository.BeginTransactionAsync())
                {
                    try
                    {
                        await _newsRepository.UpdateAsync(news);

                        //tạo category
                        var category = new NewsCategory();
                        if (categoryId != null)
                        {
                            category = await _categoryRepository
                                .FindByCondition(x => x.Id == categoryId && x.Type == news.Type)
                                .FirstOrDefaultAsync();
                            if (category == null)
                            {
                                await _newsRepository.RollbackTransactionAsync();
                                return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "categoryId_ERR_INVALID_VALUE");
                            }

                            var newsCategoryDB = await _newsCategoryMappingRepository
                                .FindByCondition(x => x.NewsId == news.Id && x.DeletedDate == null)
                                .FirstOrDefaultAsync();
                            if (newsCategoryDB == null)
                            {
                                newsCategoryDB = new NewsCategoryMapping()
                                {
                                    Id = Guid.NewGuid(),
                                    CategoryId = categoryId.Value,
                                    NewsId = news.Id,
                                    //CreatedBy = loginContactId,
                                    CreatedDate = DateTime.Now,
                                };
                                await _newsCategoryMappingRepository.CreateAsync(newsCategoryDB);
                            }
                            else
                            {
                                //khi đổi danh mục mới thì mới cần nhập lại
                                if (newsCategoryDB.CategoryId != categoryId)
                                {
                                    newsCategoryDB.IsDeleted = true;
                                    newsCategoryDB.DeletedDate = DateTime.Now;
                                    //newsCategoryDB.DeletedBy = loginContactId;
                                    await _newsCategoryMappingRepository.UpdateAsync(newsCategoryDB);

                                    var newsCategory = new NewsCategoryMapping()
                                    {
                                        Id = Guid.NewGuid(),
                                        CategoryId = categoryId.Value,
                                        NewsId = news.Id,
                                        //CreatedBy = loginContactId,
                                        CreatedDate = DateTime.Now,
                                    };
                                    await _newsCategoryMappingRepository.CreateAsync(newsCategory);
                                }
                            }
                        }

                        await trans.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync();
                        Logger.Error(ex);
                        return CatchErrorResponse(ex);
                    }
                }

                ////if (news.Status == 1 && news.Type == (int)NewsTypeEnum.News)
                //if (isNotify)
                //{
                //    var callEventBus = new CallEventBus(_logger);
                //    var objNoti = new DTOSendNotiNews()
                //    {
                //        Title = news.Title,
                //        Category = category?.Name,
                //        StockCode = news.StockCode,
                //        CategoryId = category?.Id,
                //        NewsId = news?.Id,
                //    };
                //    await callEventBus.SendNotiNews(objNoti, false);
                //}

                return SuccessResponse(news.Id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        /// <summary>
        /// Cập nhật thứ tự bài đăng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public async Task<BaseResponse> UpdateNewsOrder(DTONewsOrderRequest newsOrderRequest)
        //{
        //    try
        //    {
        //        var context = new CMSDbContext();
        //        SystemSettingBusiness systemSettingBusiness = new SystemSettingBusiness();

        //        //kiểm tra xem có chọn số 1 hay không
        //        if (!(newsOrderRequest.Details.Any(x => x.DisplayOrder == 1)))
        //            return BadRequestResponse(ErrorCodeConst.ERROR_NEWS_FIRST_NOT_FOUND, "firstOrder_ERR_NOT_FOUND");

        //        //kiểm tra xem có gửi trùng vị trí hay không
        //        if ((newsOrderRequest.Details.Select(x => x.DisplayOrder).Distinct().Count()) != newsOrderRequest.Details.Count())
        //            return BadRequestResponse(ErrorCodeConst.ERROR_NEWS_DISPLAY_ORDER_DUPLICATE, "order_ERR_DUPLICATE");

        //        //kiểm tra xem có gửi trùng code hay không
        //        if ((newsOrderRequest.Details.Select(x => x.Code).Distinct().Count()) != newsOrderRequest.Details.Count())
        //            return BadRequestResponse(ErrorCodeConst.ERROR_NEWS_CODE_DUPLICATE, "code_ERR_DUPLICATE");

        //        //kiểm tra xem chọn tối đa bao nhiêu vị trí
        //        if (newsOrderRequest.Type == NewsTypeEnum.Slider)
        //        {
        //            var systemSetting = await systemSettingBusiness.GetSystemSettingItem(Constants.LimitDisplayBanner, 1);

        //            if (systemSetting != null && !string.IsNullOrWhiteSpace(systemSetting.DisplayValue))
        //            {
        //                var limit = Int32.Parse(systemSetting.DisplayValue);
        //                if (newsOrderRequest.Details.Count > limit)
        //                    return BadRequestResponse(ErrorCodeConst.ERROR_NEWS_DISPLAY_LITMIITED, $"displayOrder_ERR_LIMIT_ORDER, Limit: {limit}");
        //            }
        //        }

        //        //update lại order các item cũ trong DB
        //        var newsDB = await _newsRepository.FindByCondition(x => x.Type == newsOrderRequest.Type && x.DeletedDate == null).ToListAsync();
        //        if (newsDB.Count > 0)
        //            newsDB.ForEach(x => x.DisplayOrder = null);

        //        //update lại order mới trong DB
        //        if (newsDB.Count > 0)
        //        {
        //            foreach (var item in newsOrderRequest.Details)
        //            {
        //                var news = newsDB.FirstOrDefault(x => x.Code == item.Code);
        //                if (news != null) news.DisplayOrder = item.DisplayOrder;
        //                else return BadRequestResponse(ErrorCodeConst.ERROR_NEWS_CODE_INVALID, $"code_ERR_INVALID, Code: {item.Code}");
        //            }
        //        }

        //        await context.SaveChangesAsync();

        //        return SuccessResponse(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        return CatchErrorResponse(ex);
        //    }
        //}

        /// <summary>
        /// Xóa bài đăng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteNews(Guid id)
        {
            try
            {
                //var loginContactId = _claimsPrincipalExtension.GetUserId();
                //if (loginContactId == Guid.Empty)
                //    return BadRequestResponse(ErrorCodeConst.ERROR_USER_NOT_FOUND, "Không tìm thấy thông tin User");

                var news = await _newsRepository.FindByCondition(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
                if (news == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Không tìm thấy tin tức hợp lệ");

                news.IsDeleted = true;
                //news.DeletedBy = loginContactId;
                news.DeletedDate = DateTime.Now;

                var newsCategory = await _newsCategoryMappingRepository.FindByCondition(x => x.NewsId == id && x.DeletedDate == null).ToListAsync();
                if (newsCategory?.Count > 0)
                {
                    newsCategory.ForEach(x =>
                    {
                        x.IsDeleted = true;
                        //x.DeletedBy = loginContactId;
                        x.DeletedDate = DateTime.Now;
                    });
                }

                using (var trans = await _newsRepository.BeginTransactionAsync())
                {
                    try
                    {
                        await _newsRepository.UpdateAsync(news);

                        if (newsCategory?.Count > 0)
                            await _newsCategoryMappingRepository.UpdateListAsync(newsCategory);

                        await trans.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync();
                        Logger.Error(ex);
                        return CatchErrorResponse(ex);
                    }
                }

                return SuccessResponse(null, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        /// <summary>
        /// Tạo slug
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string CreateSlug(string title, string code)
        {
            return StringHelper.GenerateSlug($"{title} {code}");
        }

        public async Task<BaseResponse> GetAllForWeb()
        {
            try
            {
                var today = DateTime.Now;

                // 1. Lấy news hợp lệ
                var validNews = await _newsRepository
                    .FindByCondition(n =>
                        !n.IsDeleted
                        && n.Active
                        && n.Type == NewsTypeEnum.News
                        && n.PublishDate.Date <= today.Date
                        && (
                            n.ExpireDate == null
                            || today <= n.ExpireDate
                        )
                        && (
                            !n.NeedApproved
                            || (n.NeedApproved && (n.IsApproved ?? false))
                        )
                    )
                    .AsNoTracking()
                    .Select(n => new
                    {
                        n.Id,
                        n.Title,
                        n.CreatedDate,
                        n.ThumbnailName,
                        n.Slug
                    })
                    .ToListAsync();

                if (!validNews.Any())
                {
                    return SuccessResponse(new
                    {
                        categories = new List<object>(),
                        news = new List<object>()
                    });
                }

                // 2. Mapping news - category
                var validNewsIds = validNews.Select(n => n.Id).ToList();

                var mappings = await _newsCategoryMappingRepository
                    .FindByCondition(m =>
                        !m.IsDeleted &&
                        validNewsIds.Contains(m.NewsId)
                    )
                    .AsNoTracking()
                    .Select(m => new
                    {
                        m.NewsId,
                        m.CategoryId
                    })
                    .ToListAsync();

                if (!mappings.Any())
                {
                    return SuccessResponse(new
                    {
                        categories = new List<object>(),
                        news = new List<object>()
                    });
                }

                // 3. Category có news
                var categoryIdsHasNews = mappings
                    .Select(m => m.CategoryId)
                    .Distinct()
                    .ToList();

                // 4. Lấy category (Id + Name)
                var categories = await _categoryRepository
                    .FindByCondition(c =>
                        !c.IsDeleted &&
                        categoryIdsHasNews.Contains(c.Id)
                    )
                    .AsNoTracking()
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Slug
                    })
                    .ToListAsync();

                // ➕ thêm category "Tất cả"
                var finalCategories = new List<object>();
                //{
                //    new { Id = "all", Name = "Tất cả" }
                //};
                finalCategories.AddRange(categories);

                // 5. News + CategoryId
                var news = (
                    from n in validNews
                    join m in mappings on n.Id equals m.NewsId
                    select new
                    {
                        n.Id,
                        n.Title,
                        n.CreatedDate,
                        CategoryId = m.CategoryId,
                        Slug = n.Slug,
                        SlugCategory = categories.FirstOrDefault(c => c.Id == m.CategoryId)?.Slug,
                        Thumbnail = $"{_appSettings.CloudflareR2.PublicBaseUrl}/{n.ThumbnailName}"
                    }
                ).ToList();

                return SuccessResponse(new
                {
                    categories = finalCategories,
                    news
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

    }
}