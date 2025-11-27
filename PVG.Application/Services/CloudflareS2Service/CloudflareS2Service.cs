using AutoMapper;
using Microsoft.Extensions.Options;
using PVG.Domain.Settings;

namespace PVG.Application.Services.CloudflareS2Service
{
    public class CloudflareS2Service : BaseService, ICloudflareS2Service
    {
        public CloudflareS2Service(IOptions<AppSettings> options,
            IMapper mapper) : base(options, mapper) { }

        public Task<bool> DeleteAsync(string objectKey)
        {
            throw new NotImplementedException();
        }

        public string GetPublicUrl(string objectKey)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null)
        {
            throw new NotImplementedException();
        }
    }
}