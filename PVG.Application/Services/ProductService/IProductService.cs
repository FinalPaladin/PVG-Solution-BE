using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ProductService
{
    public interface IProductService
    {
        public Task<BaseResponse> Save(RQ_SaveProductModel _input);
        public Task<BaseResponse<RS_GetAllProductModel>> GetAll();
        public Task<BaseResponse<RS_GetProductModel>> Get(RQ_GetProductModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteProductModel _input);
    }
}
