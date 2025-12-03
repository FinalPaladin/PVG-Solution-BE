using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ConfigurationService
{
    public interface IConfigurationService
    {
        public Task<BaseResponse<RS_GetAllConfigurationModel>> GetAllData();
        public Task<BaseResponse> Save(RQ_SaveConfigurationModel _input);
    }
}
