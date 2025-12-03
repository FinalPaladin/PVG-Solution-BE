using Microsoft.AspNetCore.Mvc;
using PVG.Application.Services.CloudflareR2Service;

namespace PVG.Web.Controllers
{
    [Route("api/media/image")]
    public class MediaController : PVGControllerBase
    {
        private readonly ICloudflareR2Service _cloudflareR2Service;

        public MediaController(ICloudflareR2Service cloudflareR2Service)
        {
            _cloudflareR2Service = cloudflareR2Service;
        }

        [HttpPost("upload")]
        public async Task<ObjectResult> Upload(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            return ReturnData(await _cloudflareR2Service.UploadAsync(stream, file.FileName, file.ContentType));
        }

        [HttpDelete("delete")]
        public async Task<ObjectResult> Delete(string key)
            => ReturnData(await _cloudflareR2Service.DeleteAsync(key));
    }
}