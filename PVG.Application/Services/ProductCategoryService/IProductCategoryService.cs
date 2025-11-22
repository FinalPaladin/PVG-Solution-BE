using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ProductCategoryService
{
    public interface IProductCategoryService
    {
        public Task<BaseResponse> Save(RQ_SaveProductCategoryModel _input);
        public Task<BaseResponse<RS_SearchProductCategoryModel>> Search(RQ_SearchProductCategoryModel _input);
        public Task<BaseResponse<RS_GetProductCategoryModel>> Get(RQ_GetProductCategoryModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteProductCategoryModel _input);
    }
}
