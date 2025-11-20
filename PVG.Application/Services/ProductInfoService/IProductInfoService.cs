using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ProductInfoService
{
    public interface IProductInfoService
    {
        public Task<BaseResponse> Save(RQ_SaveProductInfoModel _input);
        public Task<BaseResponse<RS_GetAllProductInfoModel>> GetAll();
        public Task<BaseResponse<RS_GetProductInfoModel>> Get(RQ_GetProductInfoModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteProductInfoModel _input);
    }
}
