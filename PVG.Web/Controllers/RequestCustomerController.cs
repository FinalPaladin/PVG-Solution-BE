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
        [Route("getalldata")]
        public async Task<IActionResult> GetAllData()
        {
            var result = await requestCustomerService.GetAllData();
            return ReturnData(result);
        }

        [HttpGet]
        [Route("getdata/{_input}")]
        public async Task<IActionResult> GetData(string _input)
        {
            var result = await requestCustomerService.GetData(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveRequestCustomerModel _input)
        {
            return ReturnData(await requestCustomerService.Save(_input));
        }

        [HttpDelete]
        [Route("deletekey/{_phone}/{_key}")]
        public async Task<IActionResult> DeleteKey(string _phone, string _key)
        {
            return ReturnData(await requestCustomerService.DeleteKey(_phone, _key));
        }

        [HttpDelete]
        [Route("delete/{_phone}")]
        public async Task<IActionResult> Delete(string _phone)
        {
            return ReturnData(await requestCustomerService.Delete(_phone));
        }
    }
}
