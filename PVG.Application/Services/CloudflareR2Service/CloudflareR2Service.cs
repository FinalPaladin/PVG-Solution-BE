using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Settings;
using System.IO;
using System.Threading.Tasks;

namespace PVG.Application.Services.CloudflareR2Service
{
    public class CloudflareR2Service : BaseService, ICloudflareR2Service
    {
        private readonly IAmazonS3 _s3;

        public CloudflareR2Service(
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
            try
            {
                var key = await Upload3S(fileStream, fileName, contentType);
                if (!string.IsNullOrEmpty(key))
                    return SuccessResponse(new { 
                        publicUrl = GetPublicUrl(key),
                        keyUrl = key,
                    });
                else
                    return BadRequestResponse(ErrorCodeConst.ERROR_UPLOAD_IMAGE_FAIL, "Upload to Cloudflare R2 failed.");
            }
            catch (Exception ex)
            {
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> DeleteAsync(string objectKey)
        {
            try
            {
                var response = await _s3.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _appSettings.CloudflareR2.BucketName,
                    Key = objectKey
                });

                return SuccessResponse((int)response.HttpStatusCode < 300);
            }
            catch (Exception ex)
            {
                return CatchErrorResponse(ex);
            }
        }

        public string GetPublicUrl(string objectKey)
        {
            // Không dùng CDN, dùng base URL S3
            return $"{_appSettings.CloudflareR2.PublicBaseUrl}/{objectKey}";
        }

        private async Task<string> Upload3S(Stream fileStream, string fileName, string? contentType = null)
        {
            var ext = Path.GetExtension(fileName);
            var key = $"{Guid.NewGuid():N}{ext}";

            var request = new PutObjectRequest
            {
                BucketName = _appSettings.CloudflareR2.BucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,

                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            var response = await _s3.PutObjectAsync(request);
            if ((int)response.HttpStatusCode < 300)
            {
                return key;
            }            
            return "";
        }

        public async Task<string> UpImage(string _publicKey, IFormFile _file)
        {
            string newPubKey = "";

            if(_file == null)
            {
                return newPubKey;
            }

            if(!string.IsNullOrEmpty(_publicKey))
            {
                var keyObj = _publicKey.Substring($"{_appSettings.CloudflareR2.PublicBaseUrl}/".Length);
                await DeleteAsync(keyObj);
            }

            await using var stream = _file.OpenReadStream();
            var result = await Upload3S(stream, _file.FileName, _file.ContentType);
            if (!string.IsNullOrEmpty(result))
            {
                newPubKey = result;
            }

            return newPubKey;
        }
    }
}