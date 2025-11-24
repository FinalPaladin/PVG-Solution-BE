using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PVG.Application.Services.EmailService;
using PVG.Application.Services.UserService;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using System.Collections.Generic;
using static PVG.Domain.Enums.UserEnum;

namespace PVG.Application.Services.RequestCustomerService
{
    public class RequestCustomerService : BaseService, IRequestCustomerService
    {
        private readonly ILogger<RequestCustomerService> _logger;
        private readonly IRequestCustomerRepository _requestCustomerRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public RequestCustomerService(
            ILogger<RequestCustomerService> logger,
            IRequestCustomerRepository requestCustomerRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserService userService)
        {
            _logger = logger;
            _requestCustomerRepository = requestCustomerRepository;
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

                Guid? id = Guid.NewGuid();

                var dataUpdate = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false && x.Phone == _input.Phone && x.ProductId == _input.ProductId).ToListAsync();

                if (dataUpdate != null && dataUpdate.Count > 0)
                {
                    id = dataUpdate.FirstOrDefault().RequestCode;
                }

                var dataCreate = new List<RequestCustomer>();

                foreach (var rc in _input.Data)
                {
                    var iExist = dataUpdate.FindIndex(y => y.Key == rc.Key);
                    if (iExist >= 0)
                    {
                        dataUpdate[iExist].Value = rc.Value;
                    }
                    else
                    {
                        dataCreate.Add(
                            new RequestCustomer()
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

                                RequestCode = id,
                                Key = rc.Key,
                                Phone = _input.Phone,
                                ProductId = _input.ProductId,
                                Value = rc.Value,
                            }
                        );
                    }
                }

                if (dataCreate.Count > 0)
                {
                    string emailTitle = string.Format("Yêu cầu khách hàng số điện thoại: {0} - {1}", _input.Phone, DateTime.Now.ToString("dd/MM/yyyy"));

                    //var sendEmail = await _emailService.SendEmailRequest(emailTitle, "");

                    dataCreate.Add(
                        new RequestCustomer()
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

                            RequestCode = id,
                            Key = "IsSentEmail",
                            Phone = _input.Phone,
                            ProductId = _input.ProductId,
                            Value = "true",
                        }
                    );
                    dataCreate.Add(
                        new RequestCustomer()
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

                            RequestCode = id,
                            Key = "EmailTitle",
                            Phone = _input.Phone,
                            ProductId = _input.ProductId,
                            Value = emailTitle,
                        }
                    );
                }

                await _requestCustomerRepository.CreateListAsync(dataCreate);
                await _requestCustomerRepository.UpdateListAsync(dataUpdate);
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

                var requestCutomersEntity = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false && x.Phone == _input.Phone
                && x.RequestCode == _input.RequestCode
                && x.ProductId == _input.ProductId).ToListAsync();

                var requestCutomers = _mapper.Map<List<RequestCustomerModel>>(requestCutomersEntity);

                var data = requestCutomers
                    .GroupBy(x => new { x.RequestCode, x.Phone, x.ProductId })
                    .Select(g => new GetRequestCustomerModel()
                    {
                        RequestCode = g.Key.RequestCode,
                        Phone = g.Key.Phone,
                        ProductId = g.Key.ProductId,
                        ListRequestCustomer = g.Select(item => new ObjRequestCustomerModel
                        {
                            Key = item.Key,
                            Value = item.Value
                        })
                        .ToList()
                    })
                    .FirstOrDefault();

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

        public async Task<BaseResponse<PaginationModel<List<GetRequestCustomerModel>>>> Search(RQ_SearchRequestCustomerModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse<PaginationModel<List<GetRequestCustomerModel>>>()
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

                var data = requestCutomers
                    .GroupBy(x => new { x.RequestCode, x.Phone, x.ProductId })
                    .Select(g => new GetRequestCustomerModel()
                    {
                        RequestCode = g.Key.RequestCode,
                        Phone = g.Key.Phone,
                        ProductId = g.Key.ProductId,
                        ListRequestCustomer = g
                        .Select(item => new ObjRequestCustomerModel
                        {
                            Key = item.Key,
                            Value = item.Value
                        })
                        .ToList()
                    })
                    .ToList();

                data.ForEach(c => c.CreatedDate = requestCutomers.FirstOrDefault(m => m.RequestCode == c.RequestCode)?.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss"));

                return new BaseResponse<PaginationModel<List<GetRequestCustomerModel>>>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy thông tin thành công",
                    Result = new()
                    {
                        Items = data,
                        PageNumber = pagination.PageNumber,
                        PerPage = pagination.PerPage,
                        TotalItems = pagination.TotalItems,
                        TotalPages = pagination.TotalPages,
                    },
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PaginationModel<List<GetRequestCustomerModel>>>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
        }

        public async Task<BaseResponse> DeleteKey(RQ_DeleteRequestCustomerModel _input)
        {
            try
            {
                var checkExist = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && x.Id == _input.Id).FirstOrDefaultAsync();

                if (checkExist == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu của khách hàng không tồn tại",
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
                await _requestCustomerRepository.UpdateAsync(checkExist);
                await _requestCustomerRepository.SaveChangesAsync();

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
                var request = await _requestCustomerRepository.FindByCondition(c => c.RequestCode == _requestCode).ToListAsync();
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