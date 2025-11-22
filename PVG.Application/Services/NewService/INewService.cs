using PVG.Core.BaseModels;
using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Application.Services.NewService
{
    public interface INewService
    {
        public Task<BaseResponse> Save(RQ_SaveNewModel _input);
        public Task<BaseResponse<RS_SearchNewModel>> Search(RQ_SearchNewModel _input);
        public Task<BaseResponse<RS_GetNewModel>> Get(RQ_GetNewModel _input);
        public Task<BaseResponse> Delete(RQ_DeleteNewModel _input);
    }
}
