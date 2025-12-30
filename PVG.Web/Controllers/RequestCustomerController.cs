using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.RequestCustomerService;
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
        [Route("delete-detail")]
        public async Task<IActionResult> DeleteDetail([FromBody] RQ_DeleteRequestCustomerModel _input)
        {
            return ReturnData(await _service.DeleteDetail(_input));
        }

        [HttpPost]
        [Route("delete")]
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

        [HttpPost]
        [Route("processed")]
        public async Task<IActionResult> Processed([FromBody] RQ_ProcessedModel _input)
             => ReturnData(await _service.Processed(_input));

        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] RQ_InserRequestCustomerModel _input)
        {
            return ReturnData(await _service.Insert(_input));
        }

        [HttpPost]
        [Route("upload-img")]
        public async Task<IActionResult> UploadImageRequestCustomer([FromForm] RQ_UploadImageRequestCustomerModel _input)
            => ReturnData(await _service.UploadImageRequestCustomer(_input));

        [HttpPost]
        [Route("remove-img")]
        public async Task<IActionResult> RemoveImageRequestCustomer([FromBody] RQ_RemoveImageRequestCustomerModel _input)
            => ReturnData(await _service.RemoveImageRequestCustomer(_input));

        [HttpPost]
        [Route("send-email")]
        public async Task<IActionResult> SendEmailRequest([FromBody] RQ_RemoveImageRequestCustomerModel _input)
            => ReturnData(await _service.SendEmailRequest(_input.RequestCode));

    }
}