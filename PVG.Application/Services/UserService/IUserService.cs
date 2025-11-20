using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.UserService
{
    public interface IUserService
    {
        public Task<BaseResponse> Login(RQ_UserLoginModel _input);
        public Task<BaseResponse<RS_GetAllUserModel>> GetAllData();
        public Task<BaseResponse<RS_GetUserModel>> GetUser(RQ_GetUserModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteUserModel _input);
    }
}
