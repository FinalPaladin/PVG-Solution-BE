using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Ocsp;
using PVG.Application.Services.CloudflareR2Service;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.RecaptchaService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ImageRequestRepository;
using PVG.Infrastucture.Repositories.ProductRepository;
using PVG.Infrastucture.Repositories.RequestCustomerDetailRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.Json;
using static PVG.Domain.Enums.UserEnum;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IUserRepository _userRepository;
        private readonly IRecaptchaService _recaptchaService;
        private readonly IProductRepository _productRepository;

        public RequestCustomerService(
            IOptions<AppSettings> options,
            IMapper mapper,
            ILogger<RequestCustomerService> logger,
            IRequestCustomerRepository requestCustomerRepository,
            IRequestCustomerDetailRepository requestCustomerDetailRepository,
            IEmailService emailService,
            IUserService userService,
            ICloudflareR2Service cloudflareR2Service,
            IImageRequestRepository imageRequestRepository,
            IUserRepository userRepository,
            IRecaptchaService recaptchaService,
            IProductRepository productRepository) : base(options, mapper)
        {
            _logger = logger;
            _requestCustomerRepository = requestCustomerRepository;
            _requestCustomerDetailRepository = requestCustomerDetailRepository;
            _emailService = emailService;
            _cloudflareR2Service = cloudflareR2Service;
            _imageRequestRepository = imageRequestRepository;
            _userRepository = userRepository;
            _recaptchaService = recaptchaService;
            _productRepository = productRepository;
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

                if (_input.Data == null || string.IsNullOrEmpty(_input.Data))
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

                    await _requestCustomerRepository.EditAsync(dataUpdate);
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

                List<SaveRequestCustomerModel> dataRC = JsonSerializer.Deserialize<List<SaveRequestCustomerModel>>(_input.Data);
                foreach (var ddu in dataRC)
                {
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

                //if (detailCreate.Count > 0)
                //{
                //    string emailTitle = string.Format("[Yêu cầu từ khách hàng SĐT: {0}, ngày: {1}", _input.Phone, DateTime.Now.ToString("dd/MM/yyyy"));

                //    List<IFormFile> attacheds = new List<IFormFile>();

                //    foreach(var item in _input.DataImage)
                //    {
                //        attacheds.Add(item.ImgFile);
                //    }

                //    var sendEmail = await _emailService.SendEmailRequest(emailTitle, GenBodyEmail(_input.FullName, _input.Phone, dataRC), attacheds);

                //    detailCreate.Add(
                //        new RequestCustomerDetail()
                //        {
                //            Id = Guid.NewGuid(),
                //            CreatedBy = null,
                //            CreatedByName = "",
                //            CreatedDate = DateTime.Now,
                //            DeletedBy = null,
                //            DeletedByName = "",
                //            DeletedDate = DateTime.Now,
                //            IsDeleted = false,
                //            ModifiedBy = null,
                //            ModifiedByName = "",
                //            ModifiedDate = DateTime.Now,

                //            RequestCode = requestCode,
                //            Key = "IsSentEmail",
                //            Value = "true",
                //        }
                //    );
                //    detailCreate.Add(
                //        new RequestCustomerDetail()
                //        {
                //            Id = Guid.NewGuid(),
                //            CreatedBy = null,
                //            CreatedByName = "",
                //            CreatedDate = DateTime.Now,
                //            DeletedBy = null,
                //            DeletedByName = "",
                //            DeletedDate = DateTime.Now,
                //            IsDeleted = false,
                //            ModifiedBy = null,
                //            ModifiedByName = "",
                //            ModifiedDate = DateTime.Now,

                //            RequestCode = requestCode,
                //            Key = "EmailTitle",
                //            Value = emailTitle,
                //        }
                //    );
                //}

                await _requestCustomerDetailRepository.UpdateListAsync(detailUpdate);
                await _requestCustomerDetailRepository.CreateListAsync(detailCreate);
                await _requestCustomerDetailRepository.SaveChangesAsync();
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

                var requestCutomerEntity = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                && x.RequestCode == _input.RequestCode).FirstOrDefaultAsync();

                if (requestCutomerEntity == null)
                    return new BaseResponse<RS_GetRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Yêu cầu khách hàng không tồn tại"
                    };

                var data = _mapper.Map<RequestCustomerModel>(requestCutomerEntity);

                var product = _productRepository.FindByCondition(x => x.Id == data.ProductId).FirstOrDefault();

                if(product != null)
                {
                    data.ProductName = product.Name;
                }

                var requestCustomerDetailEntity = await _requestCustomerDetailRepository.FindByCondition(x => !x.IsDeleted
                && x.RequestCode == _input.RequestCode).ToListAsync();

                var details = new List<RequestCustomerDetailModel>();

                if(requestCustomerDetailEntity != null)
                    details = _mapper.Map<List<RequestCustomerDetailModel>>(requestCustomerDetailEntity);

                var imgs = await _imageRequestRepository.FindByCondition(c => c.RequestCode == _input.RequestCode && !c.IsDeleted).ToListAsync();
                if (imgs?.Count > 0)
                    details.AddRange(imgs.Select((value, index) => new RequestCustomerDetailModel
                    {
                        Key = $"image{index + 1}",
                        Value = $"{_appSettings.CloudflareR2.PublicBaseUrl}/{value.Url}",
                    }));

                return new BaseResponse<RS_GetRequestCustomerModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy thông tin thành công",
                    Result = new()
                    {
                        Data = data,
                        Details = details
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

        public async Task<BaseResponse<PaginationModel<RequestCustomerModel>>> Search(RQ_SearchRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<PaginationModel<RequestCustomerModel>>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                IQueryable<RequestCustomer> query = _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && (string.IsNullOrEmpty(_input.Phone) || x.Phone.Contains(_input.Phone))
                    && (_input.ProductId == null || x.ProductId == _input.ProductId)
                    //&& (_input.RequestCode == null || x.ProductId == _input.RequestCode)
                    && (string.IsNullOrEmpty(_input.FullName) || x.FullName.ToLower().Contains(_input.FullName.ToLower()))
                    && x.IsProcessed == _input.IsProcessed
                ).OrderByDescending(x => x.CreatedDate).AsQueryable();

                var pagination = await _requestCustomerRepository.OffsetPagination<RequestCustomer>(query, _input.Page, _input.PageSize);

                var requestCutomers = _mapper.Map<List<RequestCustomerModel>>(pagination.Items);

                if (requestCutomers == null)
                    requestCutomers = new();

                //data.ForEach(c => c.CreatedDate = requestCutomers.FirstOrDefault(m => m.RequestCode == c.RequestCode)?.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss"));

                return new BaseResponse<PaginationModel<RequestCustomerModel>>()
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
                return new BaseResponse<PaginationModel<RequestCustomerModel>>()
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
                && x.Id == _input.IdDetail).FirstOrDefaultAsync();

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
                    && x.RequestCode == _input.RequestCode).FirstOrDefaultAsync();

                if (checkExist == null)
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


                checkExist.IsDeleted = true;
                checkExist.DeletedBy = isAdmin.Result.Id;
                checkExist.DeletedDate = DateTime.Now;

                await _requestCustomerRepository.EditAsync(checkExist);
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

        private string GenBodyEmail(string _fullName, string _phone, List<RequestCustomerDetailModel> _data)
        {
            string title = @"<tr>
                                <td colspan='2' style='border: 1px solid; font-weight: bold; padding: 6px 0; background-color: #36C920; padding:5px; width: 40%;'><b>{0}</b></td>
                            </tr>";
            string row = @"
                            <tr>
                                <td style='border: 1px solid; font-weight: bold; padding: 6px 0; background-color: #B5FFC0; padding:5px; width: 40%;'>{0}:</td>
                                <td style='border: 1px solid; padding: 5px;'>{1}</td>
                            </tr>
                        ";

            string content = "";

            foreach (var item in ConstRequestCustomer.ListRC)
            {
                switch (item.Value)
                {
                    case "title":
                        content += string.Format(title, item.Name);
                        break;
                    default:
                        content += string.Format(row, item.Name, GetValueByKey(_data, item.Value));
                        break;
                }                
            }

            string result = string.Format(@"
                    <h4>Chi tiết yêu cầu vay từ khách hàng</h4>
                    <table style='border: 1px solid; max-width: 80%; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;'>{0}</table>
                    ", content);
            return result;
        }

        private string GetValueByKey(List<RequestCustomerDetailModel> _data, string _key)
        {
            var obj = _data.Find(x => x.Key.ToLower() == _key.ToLower());

            if (obj != null)
                return obj.Value;
            return "";
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

        public async Task<BaseResponse<byte[]>> ExportExcel(RQ_SearchRequestCustomerModel _input)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("PVG Solution");
                byte[] fileBytes = null;

                var headers = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && (string.IsNullOrEmpty(_input.Phone) || x.Phone.Contains(_input.Phone))
                    && (_input.ProductId == null || x.ProductId == _input.ProductId)
                    && (_input.RequestCode == null || x.ProductId == _input.RequestCode)
                    && (string.IsNullOrEmpty(_input.FullName) || x.FullName.ToLower().Contains(_input.FullName.ToLower()))
                    && x.IsProcessed == _input.IsProcessed
                ).OrderByDescending(x => x.CreatedDate).ToListAsync();

                var requestCodes = headers.Select(x => x.RequestCode).ToList();

                var details = await _requestCustomerDetailRepository.FindByCondition(x => requestCodes.Contains(x.RequestCode)).ToListAsync();

                using (ExcelPackage package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Report");

                    ExcelHeaderTop(ws, "A1:J1");
                    ExcelHeaderTop(ws, "K1:M1");
                    ExcelHeaderTop(ws, "N1:S1");
                    ExcelHeaderTop(ws, "T1:Z1");
                    ExcelHeaderTop(ws, "AA1:AA1");

                    ws.Cells["A1"].Value = ConstRequestCustomer.RC_PersonalInformation_Name;
                    ws.Cells["K1"].Value = ConstRequestCustomer.RC_ContactInformation_Name;
                    ws.Cells["N1"].Value = ConstRequestCustomer.RC_JobInformation_Name;
                    ws.Cells["T1"].Value = ConstRequestCustomer.RC_CreditInformation_Name;
                    ws.Cells["AA1"].Value = ConstRequestCustomer.RC_OtherInformation_Name;

                    var headerReports = ConstRequestCustomer.ListRC.Where(x => x.Value != "title").ToList();

                    int row = 2;
                    for (int i = 0; i < headerReports.Count; i++)
                    {
                        ExcelHeader(ws, row, i + 1);
                        ws.Cells[row, i + 1].Value = headerReports[i].Name;
                    }

                    foreach (var head in headers)
                    {
                        row++;
                        var detail = details.Where(x => x.RequestCode == head.RequestCode && !x.IsDeleted).ToList();
                        for (int i = 0; i < headerReports.Count; i++)
                        {
                            string value = "";
                            var dt = detail.Find(x => x.Key.ToLower() == headerReports[i].Value.ToLower());
                            if (dt != null)
                                value = dt.Value;
                            ExcelBody(ws, row, i + 1);
                            ws.Cells[row, i + 1].Value = value;
                        }
                    }

                    fileBytes = package.GetAsByteArray();
                }

                return new BaseResponse<byte[]>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Result = fileBytes
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<byte[]>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        private void ExcelHeaderTop(ExcelWorksheet ws, string _colMerge)
        {
            string bgColor = "#36C920";
            var header = ws.Cells[_colMerge];
            header.Merge = true;
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 102, 204));
            header.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColor));
            header.Style.Font.Color.SetColor(Color.White);
            header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            header.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            header.Style.Font.Size = 12;
            header.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            header.Style.Font.Bold = true;
        }

        private void ExcelHeader(ExcelWorksheet ws, int _row, int _col)
        {
            var bgColor = "#B5FFC0";
            var header = ws.Cells[_row, _col];
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgColor));
            header.Style.Font.Color.SetColor(Color.Black);
            header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            header.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            header.Style.Font.Size = 12;
            header.Style.Font.Bold = true;
            header.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            header.AutoFitColumns();
        }
        
        private void ExcelBody(ExcelWorksheet ws, int _row, int _col)
        {
            var header = ws.Cells[_row, _col];
            header.Style.Font.Size = 12;
            header.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            header.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            header.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            header.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Nếu muốn đổi màu border
            header.Style.Border.Top.Color.SetColor(Color.Black);
            header.Style.Border.Bottom.Color.SetColor(Color.Black);
            header.Style.Border.Left.Color.SetColor(Color.Black);
            header.Style.Border.Right.Color.SetColor(Color.Black);
        }

        public async Task<BaseResponse> Processed(RQ_ProcessedModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Dữ liệu truyền vào không đúng",
                    };
                }

                var request = await _requestCustomerRepository.FindByCondition(x => x.RequestCode == _input.RequestCode).FirstOrDefaultAsync();

                if (request == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu không tồn tại",
                    };
                }

                if (request.IsDeleted)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu đã bị xóa",
                    };
                }

                if (request.IsProcessed)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu đã được hoàn tất trước đó",
                    };
                }

                var user = await _userRepository.FindByCondition(x => x.UserName == _input.UserName).FirstOrDefaultAsync();

                request.IsProcessed = true;
                request.ModifiedByName = user.FullName;
                request.ModifiedBy = user.Id;
                request.ModifiedDate = DateTime.Now;

                await _requestCustomerRepository.EditAsync(request);
                await _requestCustomerRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xử lý yêu cầu khách hàng thành công",
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

        public async Task<BaseResponse<RS_InserRequestCustomerModel>> Insert(RQ_InserRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_InserRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (_input.Data == null)
                {
                    return new BaseResponse<RS_InserRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (_input.ProductId == null || string.IsNullOrEmpty(_input.Phone))
                {
                    return new BaseResponse<RS_InserRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (await _recaptchaService.Verify(_input.Token))
                {
                    return new BaseResponse<RS_InserRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Xác thực captcha thất bại",
                    };
                }

                Guid requestCode = Guid.NewGuid();
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
                await _requestCustomerRepository.SaveChangesAsync();

                List<RequestCustomerDetail> dataDetailCreate = new();

                if (_input.Data != null && _input.Data.Count > 0)
                {
                    foreach (var dt in _input.Data)
                    {
                        RequestCustomerDetail data = new RequestCustomerDetail()
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
                            Key = dt.Key,
                            Value = dt.Value
                        };
                        dataDetailCreate.Add(data);
                    }
                }

                await _requestCustomerDetailRepository.CreateListAsync(dataDetailCreate);
                await _requestCustomerDetailRepository.SaveChangesAsync();

                return new BaseResponse<RS_InserRequestCustomerModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lưu dữ liệu thành công",
                    Result = new()
                    {
                        RequestCode = requestCode
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_InserRequestCustomerModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> SendEmailRequest(Guid _requestCode)
        {
            try
            {
                var request = await _requestCustomerRepository.FindByCondition(x => x.RequestCode == _requestCode).FirstOrDefaultAsync();

                if (request == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu không tồn tại",
                    };
                }

                string emailTitle = string.Format("[Yêu cầu từ khách hàng SĐT: {0}, ngày: {1}", request.Phone, DateTime.Now.ToString("dd/MM/yyyy"));

                List<IFormFile> attacheds = new List<IFormFile>();

                var images = await _imageRequestRepository.FindByCondition(x => x.RequestCode == _requestCode).ToListAsync();

                if(images != null)
                {
                    foreach (var img in images)
                    {
                        string publicUrl = _cloudflareR2Service.GetPublicUrl(img.Url);

                        if (string.IsNullOrEmpty(publicUrl))
                        { continue; }

                        var fileImg = await _cloudflareR2Service.GetImageAsFormFile(publicUrl);

                        attacheds.Add(fileImg);
                    }
                }

                var details = await _requestCustomerDetailRepository.FindByCondition(x => x.RequestCode == _requestCode).ToListAsync();

                var detailModel = _mapper.Map<List<RequestCustomerDetailModel>>(details);

                var sendEmail = await _emailService.SendEmailRequest(emailTitle, GenBodyEmail(request.FullName, request.Phone, detailModel), attacheds);

                if(sendEmail == null || !sendEmail.IsSuccessed)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = sendEmail.ErrMsg,
                    };
                }

                List<RequestCustomerDetail> dataDetailCreate = new()
                {
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

                        RequestCode = _requestCode,
                        Key = "IsSentEmail",
                        Value = "true",
                    },
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

                        RequestCode = _requestCode,
                        Key = "EmailTitle",
                        Value = emailTitle,
                    }
                };

                await _requestCustomerDetailRepository.CreateListAsync(dataDetailCreate);
                await _requestCustomerDetailRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Gửi yêu cầu thành công",
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

        public async Task<BaseResponse<RS_UploadImageRequestCustomerModel>> UploadImageRequestCustomer(RQ_UploadImageRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<RS_UploadImageRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                if (_input.RequestCode == null)
                {
                    return new BaseResponse<RS_UploadImageRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                var img = await _cloudflareR2Service.UpImage("", _input.ImgFile);

                if (string.IsNullOrEmpty(img))
                {
                    return new BaseResponse<RS_UploadImageRequestCustomerModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Tải ảnh lên thất bại"
                    };
                }

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
                    RequestCode = _input.RequestCode,
                    Url = img,
                });
                await _imageRequestRepository.SaveChangesAsync();

                return new BaseResponse<RS_UploadImageRequestCustomerModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tải hình ảnh thành công",
                    Result = new()
                    {
                        Key = img,
                        PublicUrl = _cloudflareR2Service.GetPublicUrl(img)
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_UploadImageRequestCustomerModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> RemoveImageRequestCustomer(RQ_RemoveImageRequestCustomerModel _input)
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

                if (_input.RequestCode == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                var deleteimg = await _cloudflareR2Service.DeleteAsync(_input.Key);

                var img = await _imageRequestRepository.FindByCondition(x => x.RequestCode == _input.RequestCode && x.Url == _input.Key).FirstOrDefaultAsync();

                await _imageRequestRepository.DeleteAsync(img);
                await _imageRequestRepository.SaveChangesAsync();


                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Xóa hình ảnh thành công",
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
    }
}