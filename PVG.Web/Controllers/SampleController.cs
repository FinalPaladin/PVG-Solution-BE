using Microsoft.AspNetCore.Mvc;
using PVG.Core.BaseModels;

namespace PVG.Web.Controllers
{
    public class SampleController : PVGControllerBase
    {
        private readonly ILogger<SampleController> _logger;

        public SampleController(ILogger<SampleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            var response = new BaseResponse
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Test successful",
                Result = new { Data = "Sample data" }
            };
            return ReturnData(response);
        }
    }
}
