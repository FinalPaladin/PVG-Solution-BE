using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.ConfigurationService;
using PVG.Application.Services.RequestCustomerService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System.Threading.Tasks;

namespace PVG.Web.Controllers
{
    [Route("api/configuration")]
    [ApiController]
    public class ConfigurationController : PVGControllerBase
    {
        IConfigurationService _service;
        public ConfigurationController(IConfigurationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllData()
        {
            var result = await _service.GetAllData();
            return ReturnData(result);
        }


        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveConfigurationModel _input)
        {
            return ReturnData(await _service.Save(_input));
        }
    }
}
