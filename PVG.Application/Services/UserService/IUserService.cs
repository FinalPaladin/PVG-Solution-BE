using PVG.Core.BaseModels;
using PVG.Domain.Models;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.UserService
{
    public interface IUserService
    {
        Task<BaseResponse> Login(RQ_UserLoginModel _input);

        Task<BaseResponse> Logout(string userName);

        Task<BaseResponse> CreateUserAsync(string userName, string password, string fullName);

        Task<BaseResponse> ChangePasswordAsync(string userName, string currentPassword, string newPassword);

        Task<BaseResponse<RS_SearchUserModel>> Search(RQ_SearchUserModel _input);

        Task<BaseResponse<RS_GetUserModel>> Get(RQ_GetUserModel _input);

        Task<BaseResponse> Delete(RQ_DeleteUserModel _input);

        Task<BaseResponse<UserModel>> CheckAdmin(string _userName, UserAdminType _adminType);
        Task<BaseResponse> ResetPassword(string _userName, string _password);
    }
}