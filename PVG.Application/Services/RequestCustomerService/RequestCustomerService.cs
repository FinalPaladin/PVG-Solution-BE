using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;
using PVG.Application.Services.EmailService;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using PVG.Infrastucture.Repositories.UserPermissionRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Numerics;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVG.Application.Services.RequestCustomerService
{
    public class RequestCustomerService: IRequestCustomerService
    {
        private readonly IRequestCustomerRepository _requestCustomerRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userReponsitory;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        public RequestCustomerService(IRequestCustomerRepository requestCustomerRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserRepository userReponsitory,
            IPermissionRepository permissionRepository,
            IUserPermissionRepository userPermissionRepository)
        {
            _requestCustomerRepository = requestCustomerRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userReponsitory = userReponsitory;
            _permissionRepository = permissionRepository;
            _userPermissionRepository = userPermissionRepository;
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

                if(_input.ProductId == null || string.IsNullOrEmpty(_input.Phone))
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

                if(dataUpdate != null && dataUpdate.Count > 0)
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

                if(dataCreate.Count > 0)
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
                var requestCutomersEntity = _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false && x.Phone == _input.Phone 
                && x.RequestCode == _input.RequestCode
                && x.ProductId == _input.ProductId).ToList();

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

        public async Task<BaseResponse<RS_GetAllRequestCustomerModel>> GetAllData()
        {
            try
            {
                var requestCustomersEntity = _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false).ToList();

                var requestCutomers = _mapper.Map< List<RequestCustomerModel>>(requestCustomersEntity);

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

                return new BaseResponse<RS_GetAllRequestCustomerModel>()
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
                return new BaseResponse<RS_GetAllRequestCustomerModel>()
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

                var userEntity = _userReponsitory.FindByCondition(x => x.IsDeleted == false && x.UserName == _input.UserDelete).FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                var user = _mapper.Map<UserModel>(userEntity);

                var permissionEntity = _permissionRepository.FindByCondition(x => x.Name == "DELETE_REQUEST_CUSTOMER").FirstOrDefaultAsync();

                if (permissionEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                var permission = _mapper.Map<PermissionModel>(permissionEntity);

                var userPermissionEntity = _userPermissionRepository.FindByCondition(x => x.UserId == user.Id && x.PermissionId == permission.Id).FirstOrDefaultAsync();

                if (userPermissionEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                checkExist.IsDeleted = true;
                checkExist.DeletedBy = user.Id;
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
                var checkExist = _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && x.RequestCode == _input.RequestCode 
                    && x.Phone == _input.Phone
                    && x.ProductId == _input.ProductId).ToList();

                if (checkExist == null || checkExist.Count == 0)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Yêu cầu của khách hàng Không tồn tại",
                    };
                }

                var userEntity = _userReponsitory.FindByCondition(x => x.IsDeleted == false && x.UserName == _input.UserDelete).FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                var user = _mapper.Map<UserModel>(userEntity);

                var permissionEntity = _permissionRepository.FindByCondition(x => x.Name == "DELETE_REQUEST_CUSTOMER").FirstOrDefaultAsync();

                if (permissionEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Phải là System Admin mới đủ quyền xóa",
                    };
                }

                var permission = _mapper.Map<PermissionModel>(permissionEntity);

                var userPermissionEntity = _userPermissionRepository.FindByCondition(x => x.UserId == user.Id && x.PermissionId == permission.Id).FirstOrDefaultAsync();

                if (userPermissionEntity == null)
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
                    rc.DeletedBy = user.Id;
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
    }
}
