using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.RequestCustomerService
{
    public interface IRequestCustomerService
    {
        public Task<BaseResponse> Save(RQ_SaveRequestCustomerModel _input);
        public Task<BaseResponse<RS_GetRequestCustomerModel>> GetData(RQ_GetRequestCustomerModel _input);
        public Task<BaseResponse<RS_GetAllRequestCustomerModel>> GetAllData();
        public Task<BaseResponse> DeleteKey(RQ_DeleteRequestCustomerModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteRequestCustomerModel _input);
    }
}
