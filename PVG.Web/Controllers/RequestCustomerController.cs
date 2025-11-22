using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/request")]
    [ApiController]
    public class RequestCustomerController: PVGControllerBase
    {
        IRequestCustomerService _service;
        public RequestCustomerController(IRequestCustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(RQ_SearchRequestCustomerModel _input)
        {
            var result = await _service.Search(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> GetData([FromBody] RQ_GetRequestCustomerModel _input)
        {
            var result = await _service.GetData(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveRequestCustomerModel _input)
        {
            return ReturnData(await _service.Save(_input));
        }

        [HttpPost]
        [Route("deletekey")]
        public async Task<IActionResult> DeleteKey(RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _service.DeleteKey(_input));
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _service.Delete(_input));
        }
    }
}
