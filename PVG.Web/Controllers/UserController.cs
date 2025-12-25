using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.UserService;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : PVGControllerBase
    {
        private IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(RQ_UserLoginModel _input)
            => ReturnData(await _service.Login(_input));

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(string userName)
            => ReturnData(await _service.Logout(userName));

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RQ_RegisterUserModel _input)
            => ReturnData(await _service.CreateUserAsync(_input.UserName, _input.Password, _input.FullName));

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] RQ_ChangePasswordModel _input)
            => ReturnData(await _service.ChangePasswordAsync(_input.UserName,  _input.CurrentPassword, _input.NewPassword));

        //[HttpPost]
        //[Route("search")]
        //public async Task<IActionResult> Search(RQ_SearchUserModel _input)
        //{
        //    var result = await _service.Search(_input);
        //    return ReturnData(result);
        //}

        //[HttpPost]
        //[Route("get")]
        //public async Task<IActionResult> Get(RQ_GetUserModel _input)
        //{
        //    var result = await _service.Get(_input);
        //    return ReturnData(result);
        //}

        //[HttpPost]
        //[Route("delete")]
        //public async Task<IActionResult> Delete(RQ_DeleteUserModel _input)
        //{
        //    var result = await _service.Delete(_input);
        //    return ReturnData(result);
        //}

        [HttpGet]
        [Route("reset-password/{_username}/{_password}")]
        public async Task<IActionResult> ResetPassword(string _username, string _password)
            => ReturnData(await _service.ResetPassword(_username, _password));

    }
}