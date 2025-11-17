using Microsoft.AspNetCore.Http;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Repositories.ConfigurationRepository;
using PVG.Infrastucture.Repositories.RequestCustomerRepository;
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

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<BaseResponse<RS_GetAllConfigurationModel>> GetAllData()
        {
            try
            {
                var result = _configurationRepository.FindAll().ToList();

                //var data = AutoMapper.Mapper<ConfigurationModel>(result);

                return new BaseResponse<RS_GetAllConfigurationModel>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get data complete",
                    Result = new(),
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_GetAllConfigurationModel>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new(),
                };
            }
        }

    }
}
