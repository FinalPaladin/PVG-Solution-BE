using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVG.Application.Services.CloudflareR2Service;
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
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PVG.Application.Services.ConfigurationService
{
    public class ConfigurationService: IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userReponsitory;
        private readonly ICloudflareR2Service _cloudflareR2Service;

        public ConfigurationService(IConfigurationRepository configurationRepository,
            IMapper mapper,
            IUserRepository userReponsitory,
            ICloudflareR2Service cloudflareR2Service)
        {
            _configurationRepository = configurationRepository;
            _mapper = mapper;
            _userReponsitory = userReponsitory;
            _cloudflareR2Service = cloudflareR2Service;
        }

        public async Task<BaseResponse<ConfigurationModel>> GetByKey(string _input)
        {
            try
            {
                if (string.IsNullOrEmpty(_input))
                {
                    return new BaseResponse<ConfigurationModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }
                var configRepository = _configurationRepository.FindByCondition(x => x.IsDeleted && x.Key == _input);

                if (configRepository == null)
                {
                    return new BaseResponse<ConfigurationModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Không tìm thấy"
                    };
                }

                var data = _mapper.Map<ConfigurationModel>(configRepository);

                return new BaseResponse<ConfigurationModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Lấy thông tin thành công",
                    Result = data
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ConfigurationModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
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
                        Message = "Dữ liệu chưa được khởi tạo",
                    };
                }

                var data = _mapper.Map<List<ConfigurationModel>>(configurationsEntity);

                var configs = configurationsEntity.Where(x => x.IsImage).ToList();

                if(configs != null && configs.Count > 0)
                {
                    foreach (var item in configs)
                    {
                        var index = data.FindIndex(x => x.Key == item.Key);
                        if (index < 0)
                            continue;

                        data[index].Value = _cloudflareR2Service.GetPublicUrl(item.Value);
                    }
                }

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
                

                if (_input.Data == null || string.IsNullOrEmpty(_input.Data))
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Input empty"
                    };
                }

                var dataUpdate = await _configurationRepository.FindAll().ToListAsync();
                var dataCreate = new List<Configuration>();

                var userEntity = await _userReponsitory.FindByCondition(x => x.UserName == _input.CreateUser && x.Actived).FirstOrDefaultAsync();

                if (userEntity == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Người dùng không tồn tại",
                    };
                }

                List<ConfigurationModel> data = JsonSerializer.Deserialize<List<ConfigurationModel>>(_input.Data);
                foreach (var rc in data)
                {
                    bool isImage = false;
                    var iExist = dataUpdate.FindIndex(y => y.Key == rc.Key);

                    var updateImg = _input.DataImage.Find(x => x.Key == rc.Key);
                    if (updateImg != null)
                    {
                        isImage = true;
                        var key = await _cloudflareR2Service.UpImage(rc.Value, updateImg.ImgFile);
                        if (!string.IsNullOrEmpty(key))
                        {
                            rc.Value = key;
                        }
                        else
                        {
                            rc.Value = "";
                        }
                    }
                    else
                    {
                        if (iExist >= 0)
                        {
                            if(dataUpdate[iExist].IsImage)
                            {
                                continue;
                            }
                        }
                    }

                    if (iExist >= 0)
                    {
                        dataUpdate[iExist].IsImage = isImage;
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
                                Id = Guid.NewGuid(),
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

                                IsImage = isImage,
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
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Save data completed",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = ex.Message,
                };
            }
        }


    }
}
