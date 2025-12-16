using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.MDataService;
using PVG.Domain.Enums;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/mdata")]
    [ApiController]
    public class MDataController : PVGControllerBase
    {
        private readonly IMDataService _service;
        public MDataController(IMDataService service)
        {
            _service = service;
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetMDataGroups([FromQuery] List<MDataEnum_Group> _group)
            => ReturnData(await _service.GetMDataByGroupAsync(_group));

        [HttpPost]
        public async Task<IActionResult> CreateMData([FromBody] List<MDataModel> _input)
            => ReturnData(await _service.CreateMdata(_input, GetUserName()));

        [HttpPut]
        [Route("{_id}")]
        public async Task<IActionResult> UpdateMData(int _id, [FromBody] MDataModel _input)
            => ReturnData(await _service.UpdateMdata(_id, _input, GetUserName()));

        [HttpDelete]
        [Route("{_id}")]
        public async Task<IActionResult> RemoveMData([FromRoute] int _id)
            => ReturnData(await _service.RemoveMData(_id, GetUserName()));

        // Add this method to the MDataController class to resolve CS0103
        private string GetUserName()
        {
            return User?.Identity?.Name ?? string.Empty;
        }
    }
}
