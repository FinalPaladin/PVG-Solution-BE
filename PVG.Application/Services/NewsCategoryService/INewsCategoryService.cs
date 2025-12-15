using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.NewsCategoryService
{
    public interface INewsCategoryService
    {
        Task<BaseResponse> GetCategory(NewsCategorySearchModel input, bool isUserSite = true);

        Task<BaseResponse> GetCategoryById(Guid id);

        Task<BaseResponse> CreateCategory(CategoryModel category, string userName);

        Task<BaseResponse> UpdateCategory(Guid id, CategoryModel category, string userName);

        Task<BaseResponse> DeleteCategory(Guid id);
    }
}