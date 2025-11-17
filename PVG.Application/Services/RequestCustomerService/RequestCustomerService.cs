using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVG.Application.Services.RequestCustomerService
{
    public class RequestCustomerService: IRequestCustomerService
    {
        private readonly IRequestCustomerRepository _requestCustomerRepository;
        public RequestCustomerService(IRequestCustomerRepository requestCustomerRepository)
        {
            _requestCustomerRepository = requestCustomerRepository;
        }

        public async Task<BaseResponse> Save(RQ_SaveRequestCustomerModel _input)
        {
            try
            {
                //if(_input == null)
                //{

                //}

                //if(_input.Data == null && _input.Data.Count == 0)
                //{

                //}

                var id = Guid.NewGuid();

                var dataUpdate = _requestCustomerRepository.FindByCondition(x => x.Phone == _input.Phone).ToList();
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

                                Key = rc.Key,
                                Phone = _input.Phone,
                                Value = rc.Value,
                            }
                        );
                    }
                }

                await _requestCustomerRepository.CreateListAsync(dataCreate);
                await _requestCustomerRepository.UpdateListAsync(dataUpdate);
                await _requestCustomerRepository.SaveChangesAsync();

                return new BaseResponse
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Save data completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse<RS_GetRequestCustomerModel>> GetData(string _input)
        {
            try
            {
                var result = _requestCustomerRepository.FindByCondition(x => x.Phone == _input).ToList();

                var requestCutomers = new List<RequestCustomerModel>();

                foreach (var rc in result)
                {
                    requestCutomers.Add(
                        new RequestCustomerModel()
                        {
                            Key = rc.Key,
                            Value = rc.Value,
                        }
                    );
                }

                var data = new GetRequestCustomerModel()
                {
                    Phone = _input,
                    ListRequestCustomer = requestCutomers
                };

                return new BaseResponse<RS_GetRequestCustomerModel>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get data complete",
                    Result = new()
                    {
                        Data = data
                    },
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetRequestCustomerModel>
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
                var result = _requestCustomerRepository.FindAll().ToList();

                var requestCutomers = new List<RequestCustomerModel>();

                foreach (var rc in result)
                {
                    requestCutomers.Add(
                        new RequestCustomerModel()
                        {
                            Key = rc.Key,
                            Value = rc.Value,
                        }    
                    );
                }

                var data = result
                    .GroupBy(x => x.Phone)
                    .Select(g => new GetRequestCustomerModel()
                    {
                        Phone = g.Key,
                        ListRequestCustomer = g
                        .Select(item => new RequestCustomerModel
                        {
                            Key = item.Key,
                            Value = item.Value
                        })
                        .ToList()
                    })
                    .ToList();

                return new BaseResponse<RS_GetAllRequestCustomerModel>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get data complete",
                    Result = new()
                    {
                        Data = data
                    },
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetAllRequestCustomerModel>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
        }

        public async Task<BaseResponse> DeleteKey(string _phone, string _key)
        {
            try
            {
                var checkExist = await _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false 
                    && x.Phone == _phone 
                    && x.Key == _key).FirstOrDefaultAsync();

                if (checkExist == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Object does not exist",
                    };
                }

                await _requestCustomerRepository.DeleteAsync(checkExist);
                await _requestCustomerRepository.SaveChangesAsync();

                return new BaseResponse
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Delete completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }

        public async Task<BaseResponse> Delete(string _phone)
        {
            try
            {
                var checkExist = _requestCustomerRepository.FindByCondition(x => x.IsDeleted == false
                    && x.Phone == _phone).ToList();

                if (checkExist == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Object does not exist",
                    };
                }

                await _requestCustomerRepository.DeleteListAsync(checkExist);
                await _requestCustomerRepository.SaveChangesAsync();

                return new BaseResponse
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Delete completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }
    }
}
