using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.RequestCustomerService
{
    public interface IRequestCustomerService
    {
        public Task<BaseResponse> Save(RQ_SaveRequestCustomerModel _input);

        public Task<BaseResponse<RS_GetRequestCustomerModel>> GetData(RQ_GetRequestCustomerModel _input);

        public Task<BaseResponse<PaginationModel<List<RequestCustomerModel>>>> Search(RQ_SearchRequestCustomerModel _input);

        public Task<BaseResponse> DeleteDetail(RQ_DeleteRequestCustomerModel _input);

        public Task<BaseResponse> Delete(RQ_DeleteRequestCustomerModel _input);

        Task<BaseResponse> GetRequestDetail(Guid _requestCode);

        public Task<BaseResponse<byte[]>> ExportExcel(RQ_SearchRequestCustomerModel _input);

        public Task<BaseResponse> Processed(Guid _input);
    }
}