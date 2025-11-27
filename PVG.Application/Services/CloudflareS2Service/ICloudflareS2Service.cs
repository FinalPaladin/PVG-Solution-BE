namespace PVG.Application.Services.CloudflareS2Service
{
    public interface ICloudflareS2Service
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string? contentType = null);
        Task<bool> DeleteAsync(string objectKey);
        string GetPublicUrl(string objectKey);
    }
}