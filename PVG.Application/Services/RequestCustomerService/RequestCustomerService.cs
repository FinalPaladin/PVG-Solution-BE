using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.RequestCustomerDetailRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.RequestCustomerService
{
    public class RequestCustomerService : BaseService, IRequestCustomerService
    {
        private readonly ILogger<RequestCustomerService> _logger;
        private readonly IRequestCustomerRepository _requestCustomerRepository;
        private readonly IRequestCustomerDetailRepository _requestCustomerDetailRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public RequestCustomerService(
            ILogger<RequestCustomerService> logger,
            IRequestCustomerRepository requestCustomerRepository,
            IRequestCustomerDetailRepository requestCustomerDetailRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserService userService)
        {
            _logger = logger;
            _requestCustomerRepository = requestCustomerRepository;
            _requestCustomerDetailRepository = requestCustomerDetailRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userService = userService;
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

                if (_input.Data == null && _input.Data.Count == 0)
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
                    };

                    await _requestCustomerRepository.CreateAsync(dataCreate);
                }

                List<RequestCustomerDetail> dataDetail = new List<RequestCustomerDetail>(),
                    detailUpdate = new List<RequestCustomerDetail>(),
                    detailCreate = new List<RequestCustomerDetail>();

                dataDetail = await _requestCustomerDetailRepository.FindByCondition(x => x.IsDeleted == false
                && x.RequestCode == _input.RequestCode).ToListAsync();

                if(dataDetail == null)
                {
                    dataDetail = new();
                }

                foreach (var ddu in _input.Data)
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
                    }
                    detailCreate.Add(data);
                }

                if (detailCreate.Count > 0)
                {
                    string emailTitle = string.Format("Yêu cầu khách hàng số điện thoại: {0} - {1}", _input.Phone, DateTime.Now.ToString("dd/MM/yyyy"));

                    //var sendEmail = await _emailService.SendEmailRequest(emailTitle, "");

                    detailCreate.Add(
                        new RequestCustomerDetail()
                        {
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
                    && (_input.RequestCode == null || x.ProductId == _input.RequestCode)
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

                var request = await _requestCustomerRepository.FindByCondition(c => c.RequestCode == _requestCode && listAllowShow.Contains(c.Key)).ToListAsync();
                if (request?.Count > 0)
                {
                    var data = _mapper.Map<List<RequestCustomerModel>>(request);
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