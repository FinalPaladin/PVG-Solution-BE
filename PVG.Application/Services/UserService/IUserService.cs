using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.UserService
{
    public interface IUserService
    {
        public Task<BaseResponse> Login(RQ_UserLoginModel _input);
        public Task<BaseResponse<RS_SearchUserModel>> Search(RQ_SearchUserModel _input);
        public Task<BaseResponse<RS_GetUserModel>> Get(RQ_GetUserModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteUserModel _input);
        public Task<BaseResponse<UserModel>> CheckAdmin(string _userName, UserAdminType _adminType);
    }
}
