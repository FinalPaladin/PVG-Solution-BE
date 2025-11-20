using Microsoft.AspNetCore.Http;
using PVG.Core.BaseModels;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ProductInfoRepository;
using PVG.Infrastucture.Repositories.ViewLogRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PVG.Application.Services.ViewLogService
{
    public class ViewLogService: IViewLogService
    {
        private readonly IViewLogRepository _viewLogRepository;

        public ViewLogService(IViewLogRepository viewLogRepository)
        {
            _viewLogRepository = viewLogRepository;
        }

        public async Task<BaseResponse<RS_ViewLogModel>> View(RQ_ViewLogModel _input)
        {
            try
            {
                string IP = "";
                DateTime FromDate = DateTime.Now,
                    ToDate = DateTime.Now;

                if(_input != null)
                {
                    IP = _input.IP;
                    FromDate = _input.FromDate;
                    ToDate = _input.ToDate;
                }

                var data = _viewLogRepository.FindByCondition(x =>
                    string.IsNullOrEmpty(IP) || x.IP.Contains(IP)
                    && x.CreatedDate >= FromDate
                    && x.CreatedDate <= ToDate
                ).ToList().Count();

                return new BaseResponse<RS_ViewLogModel>()
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "View data completed",
                    Result = new() { View = data }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<RS_ViewLogModel>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Result = new() { View = 0}
                };
            }
        }

        public async Task<BaseResponse> Save(RQ_SaveViewLogModel _input)
        {
            try
            {
                var id = Guid.NewGuid();

                var createNew = new ViewLog()
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

                    IP = _input?.IP == null ? "" : _input.IP,
                };

                await _viewLogRepository.CreateAsync(createNew);
                await _viewLogRepository.SaveChangesAsync();

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
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                };
            }
        }
    }
}
