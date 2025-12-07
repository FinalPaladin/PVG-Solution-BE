using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Web.Controllers
{
    [Route("api/request")]
    [ApiController]
    public class RequestCustomerController : PVGControllerBase
    {
        private IRequestCustomerService _service;

        public RequestCustomerController(IRequestCustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] RQ_SearchRequestCustomerModel _input)
        {
            var result = await _service.Search(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("getdata")]
        public async Task<IActionResult> GetData([FromBody] RQ_GetRequestCustomerModel _input)
        {
            var result = await _service.GetData(_input);
            return ReturnData(result);
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromForm] RQ_SaveRequestCustomerModel _input)
        {
            return ReturnData(await _service.Save(_input));
        }

        [HttpDelete]
        [Route("key")]
        public async Task<IActionResult> DeleteDetail([FromBody] RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _service.DeleteDetail(_input));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _service.Delete(_input));
        }

        [HttpGet]
        [Route("{requestCode}")]
        public async Task<ObjectResult> GetRequestCustomerById(Guid requestCode)
            => ReturnData(await _service.GetRequestDetail(requestCode));


        [HttpGet]
        [Route("exportexcel")]
        public async Task<IActionResult> ExportExcel([FromQuery] RQ_SearchRequestCustomerModel _input)
        {
            var data = await _service.ExportExcel(_input);
            
            var file = File(
                        data.Result,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Report.xlsx"
                    );

            return file;
        }

        [HttpGet]
        [Route("processed/{_input}")]
        public async Task<IActionResult> Processed(Guid _input)
             => ReturnData(await _service.Processed(_input));
    }
}