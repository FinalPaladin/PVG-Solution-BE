using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : PVGControllerBase
    {
        IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(RQ_UserLoginModel _input)
        {
            var result = await _userService.Login(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("getall")]
        public async Task<IActionResult> GetAllData()
        {
            var result = await _userService.GetAllData();
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> GetUser(RQ_GetUserModel _input)
        {
            var result = await _userService.GetUser(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RQ_DeleteUserModel _input)
        {
            var result = await _userService.Delete(_input);
            return ReturnData(result);
        }
    }
}
