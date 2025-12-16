using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.ProductCategoryService
{
    public interface IProductCategoryService
    {
        Task<BaseResponse> Save(RQ_SaveProductCategoryModel _input);

        Task<BaseResponse> Search(RQ_SearchProductCategoryModel _input);

        Task<BaseResponse> Get(RQ_GetProductCategoryModel _input);

        Task<BaseResponse> GetAll();

        Task<BaseResponse> Delete(Guid _id);

        Task<BaseResponse> Update(RQ_UpdateProductCategoryModel _input);
    }
}