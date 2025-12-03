using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.PermissionService
{
    public interface IPermissionService
    {
        public Task<BaseResponse> Save(RQ_SavePermissionModel _input);

        public Task<BaseResponse<RS_SearchPermissionModel>> Search(RQ_SearchPermissionModel _input);

        public Task<BaseResponse<RS_GetPermissionModel>> Get(RQ_GetPermissionModel _input);

        public Task<BaseResponse> Delete(RQ_DeletePermissionModel _input);
    }
}