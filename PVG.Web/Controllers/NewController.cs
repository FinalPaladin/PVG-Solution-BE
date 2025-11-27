using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.NewService;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/new")]
    [ApiController]
    public class NewController : PVGControllerBase
    {
        private INewService _service;

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