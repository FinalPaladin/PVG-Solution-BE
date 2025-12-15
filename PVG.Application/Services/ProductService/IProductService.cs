using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.ProductService
{
    public interface IProductService
    {
        public Task<BaseResponse> Save(RQ_SaveProductModel _input);

        public Task<BaseResponse> Search(RQ_SearchProductModel _input);

        public Task<BaseResponse> Get(RQ_GetProductModel _input);

        public Task<BaseResponse> Delete(RQ_DeleteProductModel _input);
    }
}