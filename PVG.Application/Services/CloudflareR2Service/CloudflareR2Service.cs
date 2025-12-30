using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
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

        public async Task<string> UpImage(string _key, IFormFile _file)
        {
            if(_file == null)
            {
                return "";
            }

            if(!string.IsNullOrEmpty(_key))
            {
                await DeleteAsync(_key);
            }

            await using var stream = _file.OpenReadStream();
            var result = await Upload3S(stream, _file.FileName, _file.ContentType);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return "";
        }

        public async Task<IFormFile> GetImageAsFormFile(string imageUrl)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var fileName = Path.GetFileName(imageUrl);

            return new FormFile(stream, 0, response.Content.Headers.ContentLength ?? 0,
                "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = response.Content.Headers.ContentType?.ToString()
            };
        }

        public async Task<BaseResponse<RS_CloudflareUploadListImageModel>> UploadListImage(RQ_CloudflareUploadListImageModel _input)
        {
            try
            {
                if(_input == null)
                {
                    return new BaseResponse<RS_CloudflareUploadListImageModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Dữ liệu đầu vào không hợp lệ",
                    };
                }

                List<CloudflareUploadModel> data = new();

                if(_input.DataImage != null && _input.DataImage.Count > 0)
                {
                    foreach (var img in _input.DataImage)
                    {
                        var upload = await UpImage("", img.ImgFile);
                        if(!string.IsNullOrEmpty(upload))
                        {
                            data.Add(new CloudflareUploadModel()
                            {
                                Key = upload,
                                PublicUrl = GetPublicUrl(upload)
                            });
                        }
                    }
                }

                return new BaseResponse<RS_CloudflareUploadListImageModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lưu dữ liệu thành công",
                    Result = new()
                    {
                        Data = data
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_CloudflareUploadListImageModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }
    }
}