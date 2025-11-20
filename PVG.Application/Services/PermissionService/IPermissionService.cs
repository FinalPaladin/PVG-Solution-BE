using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.PermissionService
{
    public interface IPermissionService
    {
        public Task<BaseResponse> Save(RQ_SavePermissionModel _input);
        public Task<BaseResponse<RS_GetAllPermissionModel>> GetAll();
        public Task<BaseResponse<RS_GetPermissionModel>> Get(RQ_GetPermissionModel _input);
        public Task<BaseResponse> Delete(RQ_DeletePermissionModel _input);
    }
}
