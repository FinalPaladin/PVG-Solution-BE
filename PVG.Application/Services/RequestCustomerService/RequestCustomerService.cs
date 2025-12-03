using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PVG.Application.Services.CloudflareR2Service;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ImageRequestRepository;
using PVG.Infrastucture.Repositories.RequestCustomerDetailRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using System.Text.Json;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.RequestCustomerService
{
    public class RequestCustomerService : BaseService, IRequestCustomerService
    {
        private readonly ILogger<RequestCustomerService> _logger;
        private readonly IRequestCustomerRepository _requestCustomerRepository;
        private readonly IRequestCustomerDetailRepository _requestCustomerDetailRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ICloudflareR2Service _cloudflareR2Service;
        private readonly IImageRequestRepository _imageRequestRepository;

        public RequestCustomerService(
            IOptions<AppSettings> options,
            IMapper mapper,
            ILogger<RequestCustomerService> logger,
            IRequestCustomerRepository requestCustomerRepository,
            IRequestCustomerDetailRepository requestCustomerDetailRepository,
            IEmailService emailService,
            IUserService userService,
            ICloudflareR2Service cloudflareR2Service,
            IImageRequestRepository imageRequestRepository) : base(options, mapper)
        {
            _logger = logger;
            _requestCustomerRepository = requestCustomerRepository;
            _requestCustomerDetailRepository = requestCustomerDetailRepository;
            _emailService = emailService;
            _cloudflareR2Service = cloudflareR2Service;
            _imageRequestRepository = imageRequestRepository;
        }

        public async Task<BaseResponse> Save(RQ_SaveRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (_input.Data == null)// || string.IsNullOrEmpty(_input.Data)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (_input.ProductId == null || string.IsNullOrEmpty(_input.Phone))
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                Guid? requestCode = Guid.NewGuid();
                if (_input.RequestCode != null)
                {
                    var dataUpdate = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && x.ProductId == _input.ProductId
                    && x.RequestCode == _input.RequestCode).FirstOrDefaultAsync();

                    if (dataUpdate != null)
                    {
                        requestCode = dataUpdate.RequestCode;
                    }

                    await _requestCustomerRepository.UpdateAsync(dataUpdate);
                }
                else
                {
                    var dataCreate = new RequestCustomer()
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = null,
                        CreatedByName = "",
                        CreatedDate = DateTime.Now,
                        DeletedBy = null,
                        DeletedByName = "",
                        DeletedDate = DateTime.Now,
                        IsDeleted = false,
                        ModifiedBy = null,
                        ModifiedByName = "",
                        ModifiedDate = DateTime.Now,

                        ProductId = _input.ProductId,
                        RequestCode = requestCode,
                        Phone = _input.Phone,
                        FullName = _input.FullName,
                        IsProcessed = false,
                    };

                    await _requestCustomerRepository.CreateAsync(dataCreate);
                }

                List<RequestCustomerDetail> dataDetail = new List<RequestCustomerDetail>(),
                    detailUpdate = new List<RequestCustomerDetail>(),
                    detailCreate = new List<RequestCustomerDetail>();

                dataDetail = await _requestCustomerDetailRepository.FindByCondition(x => x.IsDeleted == false
                && x.RequestCode == _input.RequestCode).ToListAsync();

                if (_input.DataImage != null && _input.DataImage.Count > 0)
                {
                    var imagesRequest = await _imageRequestRepository.FindByCondition(x => !x.IsDeleted && x.RequestCode == requestCode).ToListAsync();
                    if (imagesRequest == null || imagesRequest.Count == 0)
                    {
                        imagesRequest = new List<ImageRequest>();
                    }
                    else
                    {
                        foreach (ImageRequest ir in imagesRequest)
                        {
                            await _imageRequestRepository.DeleteAsync(ir);
                            await _cloudflareR2Service.DeleteAsync(ir.Url);
                        }
                        await _imageRequestRepository.SaveChangesAsync();
                    }

                    foreach (var img in _input.DataImage)
                    {
                        var key = await _cloudflareR2Service.UpImage("", img.ImgFile);
                        if (!string.IsNullOrEmpty(key))
                        {
                            await _imageRequestRepository.CreateAsync(new ImageRequest()
                            {
                                Id = Guid.NewGuid(),
                                CreatedBy = null,
                                CreatedByName = "",
                                CreatedDate = DateTime.Now,
                                DeletedBy = null,
                                DeletedByName = "",
                                DeletedDate = DateTime.Now,
                                IsDeleted = false,
                                ModifiedBy = null,
                                ModifiedByName = "",
                                ModifiedDate = DateTime.Now,

                                Content = "",
                                RequestCode = requestCode,
                                Url = key,
                            });
                        }
                    }
                    await _imageRequestRepository.SaveChangesAsync();
                }

                if (dataDetail == null)
                {
                    dataDetail = new();
                }

                string row = @"
                                <tr>
                                    <td style=""font-weight: bold; padding: 6px 0;"">{0}:</td>
                                    <td style=""padding: 6px 0;"">{1}</td>
                                </tr>
                            ";
                string htmlBody = @"
                                    <h4>Chi tiết yêu cầu vay từ khách hàng</h4>
                                    <table style=""width: 100%; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;"">
                                        {0}
                                    </table>
                                    ";

                string rows = "";
                List<SaveRequestCustomerModel> dataRC = JsonSerializer.Deserialize<List<SaveRequestCustomerModel>>(_input.Data);
                foreach (var ddu in dataRC)
                {
                    rows += string.Format(row, ddu.Name, ddu.Value);
                    var data = dataDetail.Find(x => x.Key == ddu.Key);
                    if (data != null)
                    {
                        data.Value = ddu.Value;
                        detailUpdate.Add(data);
                    }
                    else
                    {
                        data = new RequestCustomerDetail()
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = null,
                            CreatedByName = "",
                            CreatedDate = DateTime.Now,
                            DeletedBy = null,
                            DeletedByName = "",
                            DeletedDate = DateTime.Now,
                            IsDeleted = false,
                            ModifiedBy = null,
                            ModifiedByName = "",
                            ModifiedDate = DateTime.Now,

                            RequestCode = requestCode,
                            Key = ddu.Key,
                            Value = ddu.Value
                        };
                        detailCreate.Add(data);
                    }
                }

                if (detailCreate.Count > 0)
                {
                    string emailTitle = string.Format("[{2}]Yêu cầu từ khách hàng SĐT: {0}, ngày: {1}", _input.Phone, DateTime.Now.ToString("dd/MM/yyyy"), requestCode);

                    var sendEmail = await _emailService.SendEmailRequest(emailTitle, string.Format(htmlBody, rows));

                    detailCreate.Add(
                        new RequestCustomerDetail()
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = null,
                            CreatedByName = "",
                            CreatedDate = DateTime.Now,
                            DeletedBy = null,
                            DeletedByName = "",
                            DeletedDate = DateTime.Now,
                            IsDeleted = false,
                            ModifiedBy = null,
                            ModifiedByName = "",
                            ModifiedDate = DateTime.Now,

                            RequestCode = requestCode,
                            Key = "IsSentEmail",
                            Value = "true",
                        }
                    );
                    detailCreate.Add(
                        new RequestCustomerDetail()
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = null,
                            CreatedByName = "",
                            CreatedDate = DateTime.Now,
                            DeletedBy = null,
                            DeletedByName = "",
                            DeletedDate = DateTime.Now,
                            IsDeleted = false,
                            ModifiedBy = null,
                            ModifiedByName = "",
                            ModifiedDate = DateTime.Now,

                            RequestCode = requestCode,
                            Key = "EmailTitle",
                            Value = emailTitle,
                        }
                    );
                }

                await _requestCustomerDetailRepository.UpdateListAsync(detailUpdate);
                await _requestCustomerDetailRepository.CreateListAsync(detailCreate);
                await _requestCustomerRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lưu dữ liệu thành công",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<RS_GetRequestCustomerModel>> GetData(RQ_GetRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_GetRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                var requestCutomerEntity = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false && x.Phone == _input.Phone
                && x.RequestCode == _input.RequestCode
                && x.ProductId == _input.ProductId).FirstOrDefaultAsync();

                var data = _mapper.Map<RequestCustomerModel>(requestCutomerEntity);

                return new BaseResponse<RS_GetRequestCustomerModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy thông tin thành công",
                    Result = new()
                    {
                        Data = data
                    },
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetRequestCustomerModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
        }

        public async Task<BaseResponse<PaginationModel<List<RequestCustomerModel>>>> Search(RQ_SearchRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<PaginationModel<List<RequestCustomerModel>>>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                IQueryable<RequestCustomer> query = _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && (string.IsNullOrEmpty(_input.Phone) || x.Phone.Contains(_input.Phone))
                    && (_input.ProductId == null || x.ProductId == _input.ProductId)
                    && (_input.RequestCode == null || x.ProductId == _input.RequestCode
                    && (string.IsNullOrEmpty(_input.FullName) || x.FullName.ToLower().Contains(_input.FullName.ToLower())))
                ).OrderByDescending(x => x.CreatedDate).AsQueryable();

                var pagination = await _requestCustomerRepository.OffsetPagination<RequestCustomer>(query, _input.Page, _input.PageSize);

                var requestCutomers = _mapper.Map<List<RequestCustomerModel>>(pagination.Items);

                if (requestCutomers == null)
                    requestCutomers = new();

                //data.ForEach(c => c.CreatedDate = requestCutomers.FirstOrDefault(m => m.RequestCode == c.RequestCode)?.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss"));

                return new BaseResponse<PaginationModel<List<RequestCustomerModel>>>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy thông tin thành công",
                    Result = new()
                    {
                        Items = requestCutomers,
                        PageNumber = pagination.PageNumber,
                        PerPage = pagination.PerPage,
                        TotalItems = pagination.TotalItems,
                        TotalPages = pagination.TotalPages,
                    },
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginationModel<List<RequestCustomerModel>>>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
        }

        public async Task<BaseResponse> DeleteDetail(RQ_DeleteRequestCustomerModel _input)
        {
            try
            {
                var isAdmin = await _userService.CheckAdmin(_input.UserDelete, UserAdminType.SystemAdmin);

                if (isAdmin == null || !isAdmin.IsSuccess)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                var checkExist = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && x.RequestCode == _input.RequestCode).FirstOrDefaultAsync();

                if (checkExist == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu của khách hàng không tồn tại",
                    };
                }

                var checkExistDetail = await _requestCustomerDetailRepository.FindByCondition(x => x.RequestCode == checkExist.RequestCode
                && !x.IsDeleted
                && x.Id == _input.Id).FirstOrDefaultAsync();

                if (checkExistDetail == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu của khách hàng không tồn tại",
                    };
                }

                checkExistDetail.IsDeleted = true;
                checkExistDetail.DeletedBy = isAdmin.Result.Id;
                checkExistDetail.DeletedDate = DateTime.Now;
                await _requestCustomerDetailRepository.UpdateAsync(checkExistDetail);
                await _requestCustomerDetailRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xóa thành công",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> Delete(RQ_DeleteRequestCustomerModel _input)
        {
            try
            {
                var checkExist = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && x.RequestCode == _input.RequestCode
                    && x.Phone == _input.Phone
                    && x.ProductId == _input.ProductId).ToListAsync();

                if (checkExist == null || checkExist.Count == 0)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu của khách hàng Không tồn tại",
                    };
                }

                var isAdmin = await _userService.CheckAdmin(_input.UserDelete, UserAdminType.SystemAdmin);

                if (isAdmin == null || !isAdmin.IsSuccess)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                foreach (var rc in checkExist)
                {
                    rc.IsDeleted = true;
                    rc.DeletedBy = isAdmin.Result.Id;
                    rc.DeletedDate = DateTime.Now;
                }

                await _requestCustomerRepository.UpdateListAsync(checkExist);
                await _requestCustomerRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Delete completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> GetRequestDetail(Guid _requestCode)
        {
            try
            {
                var listAllowShow = new List<string> { "address", "phone", "fullname", "redBookAddress" };

                var detailDb = await _requestCustomerDetailRepository.FindByCondition(c => c.RequestCode == _requestCode).ToListAsync();
                if (detailDb?.Count > 0)
                {
                    var data = _mapper.Map<List<RequestCustomerDetailModel>>(detailDb.Where(c => listAllowShow.Contains(c.Key)).ToList());

                    var imgs = await _imageRequestRepository.FindByCondition(c => c.RequestCode == _requestCode && !c.IsDeleted).ToListAsync();
                    if (imgs?.Count > 0)
                        data.AddRange(imgs.Select((value, index) => new RequestCustomerDetailModel
                        {
                            Key = $"image{index + 1}",
                            Value = $"{_appSettings.CloudflareR2.PublicBaseUrl}/{value.Url}",
                        }));

                    return SuccessResponse(data);
                }
                else
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, $"request_ERR_NOT_FOUND ({ErrorCodeConst.ERROR_REQUEST_NOT_FOUND})");
            }
            catch (Exception ex)
            {
                _logger.LogError("GetRequestDetail: {0}", ex.Message);
                return CatchErrorResponse(ex);
            }
        }
    }
}