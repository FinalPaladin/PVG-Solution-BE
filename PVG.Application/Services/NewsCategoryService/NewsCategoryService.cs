using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Extensions;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Domain.Utilities;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.NewsCategoryMappingRepository;
using PVG.Infrastucture.Repositories.NewsCategoryRepository;
using PVG.Infrastucture.Repositories.NewsRepository;

namespace PVG.Application.Services.NewsCategoryService
{
    public class NewsCategoryService : BaseService, INewsCategoryService
    {
        private readonly INewsCategoryRepository _categoryRepository;
        private readonly INewsCategoryMappingRepository _newsCategoryMappingRepository;
        private readonly INewsRepository _newsRepository;

        public NewsCategoryService(
            IOptions<AppSettings> settings, 
            IMapper mapper,
            INewsCategoryRepository categoryRepository,
            INewsCategoryMappingRepository newsCategoryMappingRepository,
            INewsRepository newsRepository
            ) : base(settings, mapper)
        {
            _categoryRepository = categoryRepository;
            _newsCategoryMappingRepository = newsCategoryMappingRepository;
            _newsRepository = newsRepository;

        }

        public async Task<BaseResponse> GetCategory(NewsCategorySearchModel input, bool isUserSite = true)
        {
            try
            {
                input.Keywords = input.Keywords?.ToLower();
                var categories = await _categoryRepository.FindByCondition(m =>
                    (string.IsNullOrEmpty(input.Keywords) || m.Name.ToLower().Contains(input.Keywords))
                    && (input.Active == null || m.Status == input.Active)
                    && !m.IsDeleted).ToListAsync();

                if (categories?.Count > 0)
                {
                    var response = _mapper.Map<List<NewsCategoryResponseModel>>(categories);
                    if (isUserSite)
                        response = response.OrderBy(c => c.DisplayOrder).ToList();
                    else
                        response = response.OrderByDescending(c => c.CreatedDate).ToList();


                    if (input.IsPaging)
                        return SuccessResponse(Paging(response, input.Page, input.PageSize), "success");
                    else
                        return SuccessResponse(response, "success");
                }
                else
                    return SuccessResponse(Paging(new List<NewsCategoryResponseModel>(), input.Page, input.PageSize), "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetCategoryById(Guid id)
        {
            try
            {
                return SuccessResponse(await _categoryRepository.FindByCondition(m => m.Id == id && !m.IsDeleted).FirstOrDefaultAsync(), "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> CreateCategory(NewsCategoryModel category, string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(category.Name))
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Tên danh mục không được để trống");

                var categoryDb = await _categoryRepository.FindByCondition(m => m.Name.Equals(category.Name) && !m.IsDeleted).FirstOrDefaultAsync();
                if (categoryDb != null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Tên danh mục đã tồn tại");

                var newCategory = _mapper.Map<NewsCategory>(category);

                newCategory.Id = Guid.NewGuid();
                //newCategory.CreatedBy = _claimsPrincipalExtension.GetUserId();
                newCategory.CreatedByName = userName;
                newCategory.CreatedDate = DateTime.Now;
                newCategory.Slug = StringHelper.GenerateSlug($"{category.Name}");

                Guid newCategoryId;
                using (var transaction = await _categoryRepository.BeginTransactionAsync())
                {
                    try
                    {
                        newCategoryId = await _categoryRepository.CreateAsync(newCategory);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        await _categoryRepository.RollbackTransactionAsync();
                        Logger.Error(ex);
                        return CatchErrorResponse(ex);
                    }
                }

                return SuccessResponse(new { Id = newCategoryId }, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> UpdateCategory(Guid id, NewsCategoryModel category, string userName)
        {
            try
            {
                var categoryEntity = await _categoryRepository.FindByCondition(m => m.Id == id).FirstOrDefaultAsync();
                if (categoryEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Không tìm thấy danh mục sản phẩm");

                var updateCategory = await _categoryRepository.UpdateValueAsync(categoryEntity, category);

                updateCategory.ModifiedByName = userName;
                updateCategory.ModifiedDate = DateTime.Now;

                using (var transaction = await _categoryRepository.BeginTransactionAsync())
                {
                    try
                    {
                        await _categoryRepository.UpdateAsync(updateCategory);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        await _categoryRepository.RollbackTransactionAsync();
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

        public async Task<BaseResponse> DeleteCategory(Guid id)
        {
            try
            {
                var categoryEntity = await _categoryRepository.FindByCondition(m => m.Id == id && !m.IsDeleted).FirstOrDefaultAsync();
                if (categoryEntity == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "Không tìm thấy danh mục");

                var newsCategories = await _newsCategoryMappingRepository.FindByCondition(m => m.CategoryId == categoryEntity.Id).ToListAsync();
                if (newsCategories?.Count > 0)
                {
                    var newsIds = newsCategories.Select(m => m.NewsId).ToList();
                    var news = await _newsRepository.FindByCondition(m => newsIds.Contains(m.Id) && !m.IsDeleted && m.Active).ToListAsync();
                    if (news?.Count > 0 && categoryEntity.Status)
                        return BadRequestResponse(ErrorCodeConst.ERROR_MAPPING_ITEMS_STILL_VALID,
                            "Không thể chuyển trạng thái danh mục do tồn tại tin tức còn hiệu lực");
                }

                categoryEntity.Status = categoryEntity.Status ? false : true;
                //categoryEntity.DeletedByName = userName;
                categoryEntity.DeletedDate = DateTime.Now;

                using (var transaction = await _categoryRepository.BeginTransactionAsync())
                {
                    try
                    {
                        //update product
                        await _categoryRepository.UpdateAsync(categoryEntity);

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await _categoryRepository.RollbackTransactionAsync();
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

        public async Task<BaseResponse> GetAll()
        {
            try
            {
                var newsCategoriesDb = await _categoryRepository.FindAll().ToListAsync();
                return SuccessResponse(newsCategoriesDb.Select(c => new { Id = c.Id, Name = c.Name }).ToList(), "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }
    }
}