using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.InitPageService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Application.Services.ViewLogService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/viewlog")]
    [ApiController]
    public class ViewLogController : PVGControllerBase
    {
        IViewLogService _service;
        public ViewLogController(IViewLogService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("view")]
        public async Task<IActionResult> View(RQ_ViewLogModel _input)
        {
            var result = await _service.View(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save(RQ_SaveViewLogModel _input)
        {
            var result = await _service.Save(_input);
            return ReturnData(result);
        }

    }
}
