using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.NewService;
using PVG.Application.Services.ProductService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/new")]
    [ApiController]
    public class NewController : PVGControllerBase
    {
        INewService _service;
        public NewController(INewService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] RQ_SearchNewModel _input)
        {
            var result = await _service.Search(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get([FromBody] RQ_GetNewModel _input)
        {
            return ReturnData(await _service.Get(_input));
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveNewModel _input)
        {
            return ReturnData(await _service.Save(_input));
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Save([FromBody] RQ_DeleteNewModel _input)
        {
            return ReturnData(await _service.Delete(_input));
        }
    }
}
