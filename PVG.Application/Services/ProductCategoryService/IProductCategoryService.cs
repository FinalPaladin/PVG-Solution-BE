using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.ProductCategoryService
{
    public interface IProductCategoryService
    {
        public Task<BaseResponse> Save(RQ_SaveProductCategoryModel _input);

        public Task<BaseResponse> Search(RQ_SearchProductCategoryModel _input);

        public Task<BaseResponse> Get(RQ_GetProductCategoryModel _input);

        public Task<BaseResponse> Delete(Guid _id);

        Task<BaseResponse> Update(RQ_UpdateProductCategoryModel _input);
    }
}