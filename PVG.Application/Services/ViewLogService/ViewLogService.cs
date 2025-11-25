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
                if (_input == null)
                {
                    return new BaseResponse<RS_ViewLogModel>()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                var data = _viewLogRepository.FindByCondition(x =>
                    (string.IsNullOrEmpty(_input.IP) || x.IP == _input.IP)
                    && x.Screen == _input.Screen
                    && (x.DetailId == null || x.DetailId == _input.DetailId)
                    && x.CreatedDate >= _input.FromDate
                    && x.CreatedDate <= _input.ToDate
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
                if (_input == null)
                {
                    return new BaseResponse()
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

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
                    DetailId = _input.DetailId,
                    NumberOfTimes = 1,
                    Screen = _input.Screen
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
