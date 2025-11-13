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
        IRequestCustomerService requestCustomerService;
        public RequestCustomerController(IRequestCustomerService _requestCustomerService)
        {
            requestCustomerService = _requestCustomerService;
        }

        [HttpGet]
        [Route("GetAllData")]
        public async Task<IActionResult> GetAllData()
        {
            var result = await requestCustomerService.GetAllData();
            return ReturnData(result);
        }

        [HttpGet]
        [Route("GetData/{_input}")]
        public async Task<IActionResult> GetData(string _input)
        {
            var result = await requestCustomerService.GetData(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveRequestCustomerModel _input)
        {
            return ReturnData(await requestCustomerService.Save(_input));
        }
    }
}
