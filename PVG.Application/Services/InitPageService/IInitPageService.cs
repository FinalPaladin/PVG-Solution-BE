using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.InitPageService
{
    public interface IInitPageService
    {
        public Task<BaseResponse<ProductInitPageModel>> Product();
    }
}
