using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.InitPageService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/initpage")]
    [ApiController]
    public class InitPageController : PVGControllerBase
    {
        IInitPageService _service;
        public InitPageController(IInitPageService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("system")]
        public async Task<IActionResult> System()
        {
            var result = await _service.System();
            return ReturnData(result);
        }

        [HttpGet]
        [Route("product")]
        public async Task<IActionResult> Product()
        {
            var result = await _service.Product();
            return ReturnData(result);
        }

        [HttpGet]
        [Route("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _service.Dashboard();
            return ReturnData(result);
        }

    }
}
