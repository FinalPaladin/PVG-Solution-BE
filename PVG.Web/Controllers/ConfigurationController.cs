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
        IConfigurationService _configurationService;
        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllData()
        {
            var result = await _configurationService.GetAllData();
            return ReturnData(result);
        }


        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] RQ_SaveConfigurationModel _input)
        {
            return ReturnData(await _configurationService.Save(_input));
        }
    }
}
