using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.ViewLogService
{
    public interface IViewLogService
    {
        public Task<BaseResponse<RS_ViewLogModel>> View(RQ_ViewLogModel _input);
        public Task<BaseResponse> Save(RQ_SaveViewLogModel _input);
    }
}
