using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.ProductService
{
    public interface IProductService
    {
        Task<BaseResponse> Search(ProductSearchRequest _input);

        Task<BaseResponse> GetById(Guid _id);

        Task<BaseResponse> Create(ProductCreateRequest _input);

        Task<BaseResponse> Update(Guid _id, ProductUpdateRequest _input);

        Task<BaseResponse> Delete(Guid _id, string _userName);

        Task<BaseResponse> InitProductsApp();
    }
}