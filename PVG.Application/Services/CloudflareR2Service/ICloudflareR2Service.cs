using Microsoft.AspNetCore.Http;
using PVG.Core.BaseModels;
using PVG.Domain.Models;

namespace PVG.Application.Services.CloudflareR2Service
{
    public interface ICloudflareR2Service
    {
        Task<BaseResponse> UploadAsync(Stream fileStream, string fileName, string? contentType = null);
        Task<BaseResponse> DeleteAsync(string objectKey);
        string GetPublicUrl(string objectKey);
        public Task<string> UpImage(string _publicKey, IFormFile _file);
        public Task<IFormFile> GetImageAsFormFile(string imageUrl);
        public Task<BaseResponse<RS_CloudflareUploadListImageModel>> UploadListImage(RQ_CloudflareUploadListImageModel _input);
    }
}