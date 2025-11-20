using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/request_customer")]
    [ApiController]
    public class RequestCustomerController: PVGControllerBase
    {
        IRequestCustomerService _requestCustomerService;
        public RequestCustomerController(IRequestCustomerService requestCustomerService)
        {
            _requestCustomerService = requestCustomerService;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllData()
        {
            var result = await _requestCustomerService.GetAllData();
            return ReturnData(result);
        }

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> GetData([FromBody] RQ_GetRequestCustomerModel _input)
        {
            var result = await _requestCustomerService.GetData(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveRequestCustomerModel _input)
        {
            return ReturnData(await _requestCustomerService.Save(_input));
        }

        [HttpPost]
        [Route("deletekey")]
        public async Task<IActionResult> DeleteKey(RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _requestCustomerService.DeleteKey(_input));
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _requestCustomerService.Delete(_input));
        }
    }
}
