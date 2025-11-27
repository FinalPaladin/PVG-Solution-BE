using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Settings;

namespace PVG.Application.Services.CloudflareS2Service
{
    public class CloudflareS2Service : BaseService, ICloudflareS2Service
    {
        private readonly IAmazonS3 _s3;

        public CloudflareS2Service(
            IOptions<AppSettings> options,
            IMapper mapper
            ) : base(options, mapper) 
        {
            var config = new AmazonS3Config
            {
                ServiceURL = options.Value.CloudflareR2.S3APIUrl,
                ForcePathStyle = true
            };

            _s3 = new AmazonS3Client(
                options.Value.CloudflareR2.AccessKeyId,
                options.Value.CloudflareR2.SecretAccessKey,
                config);
        }

        public async Task<BaseResponse> UploadAsync(Stream fileStream, string fileName, string? contentType = null)
        {
            var ext = Path.GetExtension(fileName);
            var key = $"{Guid.NewGuid():N}{ext}";

            var request = new PutObjectRequest
            {
                BucketName = _appSettings.CloudflareR2.BucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType
            };

            var response = await _s3.PutObjectAsync(request);

            if ((int)response.HttpStatusCode < 300)
                return SuccessResponse(GetPublicUrl(key));
            else
                return BadRequestResponse(ErrorCodeConst.ERROR_UPLOAD_IMAGE_FAIL, "Upload to Cloudflare R2 failed.");
        }

        public async Task<BaseResponse> DeleteAsync(string objectKey)
        {
            var response = await _s3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _appSettings.CloudflareR2.BucketName,
                Key = objectKey
            });

            return SuccessResponse((int)response.HttpStatusCode < 300);
        }

        public Task<BaseResponse> GetPublicUrl(string objectKey)
        {
            // Không dùng CDN, dùng base URL S3
            return Task.FromResult(SuccessResponse($"{_appSettings.CloudflareR2.PublicBaseUrl}/{objectKey}"));
        }
    }
}