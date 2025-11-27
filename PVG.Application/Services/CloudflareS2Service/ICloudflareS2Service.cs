using PVG.Core.BaseModels;

namespace PVG.Application.Services.CloudflareS2Service
{
    public interface ICloudflareS2Service
    {
        Task<BaseResponse> UploadAsync(Stream fileStream, string fileName, string? contentType = null);
        Task<BaseResponse> DeleteAsync(string objectKey);
        Task<BaseResponse> GetPublicUrl(string objectKey);
    }
}