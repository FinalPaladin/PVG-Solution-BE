using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ConfigurationService
{
    public class ConfigurationService: IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userReponsitory;

        public ConfigurationService(IConfigurationRepository configurationRepository,
            IMapper mapper,
            IUserRepository userReponsitory)
        {
            _configurationRepository = configurationRepository;
            _mapper = mapper;
            _userReponsitory = userReponsitory;
        }

        public async Task<BaseResponse<RS_GetAllConfigurationModel>> GetAllData()
        {
            try
            {
                var configurationsEntity = await _configurationRepository.FindAll().ToListAsync();

                if(configurationsEntity == null || configurationsEntity.Count == 0)
                {
                    return new BaseResponse<RS_GetAllConfigurationModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Data not found",
                    };
                }

                var data = _mapper.Map<List<ConfigurationModel>>(configurationsEntity);

                return new BaseResponse<RS_GetAllConfigurationModel>()
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
                return new BaseResponse<RS_GetAllConfigurationModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
        }

        public async Task<BaseResponse> Save(RQ_SaveConfigurationModel _input)
        {
            try
            {
                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Input empty"
                    };
                }
                

                if (_input.Data == null && _input?.Data.Count == 0)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Input empty"
                    };
                }

                var id = Guid.NewGuid();

                var dataUpdate = await _configurationRepository.FindAll().ToListAsync();
                var dataCreate = new List<Configuration>();

                var userEntity = await _userReponsitory.FindByCondition(x => x.Id == _input.CreateUserId).FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Người dùng không tồn tại",
                    };
                }

                foreach (var rc in _input.Data)
                {
                    var iExist = dataUpdate.FindIndex(y => y.Key == rc.Key);
                    if (iExist >= 0)
                    {
                        dataUpdate[iExist].Value = rc.Value;
                        dataUpdate[iExist].ModifiedBy = userEntity.Id;
                        dataUpdate[iExist].ModifiedByName = userEntity.FullName;
                        dataUpdate[iExist].ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        dataCreate.Add(
                            new Configuration()
                            {
                                CreatedBy = userEntity.Id,
                                CreatedByName = userEntity.FullName,
                                CreatedDate = DateTime.Now,
                                DeletedBy = null,
                                DeletedByName = "",
                                DeletedDate = DateTime.Now,
                                IsDeleted = false,
                                ModifiedBy = null,
                                ModifiedByName = "",
                                ModifiedDate = DateTime.Now,

                                Key = rc.Key,
                                Value = rc.Value,
                            }
                        );
                    }
                }

                await _configurationRepository.CreateListAsync(dataCreate);
                await _configurationRepository.UpdateListAsync(dataUpdate);
                await _configurationRepository.SaveChangesAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Save data completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status200OK,
                    Message = ex.Message,
                };
            }
        }

    }
}
