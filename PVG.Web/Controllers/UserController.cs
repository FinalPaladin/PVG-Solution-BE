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
        IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(RQ_UserLoginModel _input)
        {
            var result = await _service.Login(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(RQ_SearchUserModel _input)
        {
            var result = await _service.Search(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get(RQ_GetUserModel _input)
        {
            var result = await _service.Get(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RQ_DeleteUserModel _input)
        {
            var result = await _service.Delete(_input);
            return ReturnData(result);
        }
    }
}
