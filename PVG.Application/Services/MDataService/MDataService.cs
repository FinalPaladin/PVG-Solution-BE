using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Enums;
using PVG.Domain.Extensions;
using PVG.Domain.Models;
using PVG.Domain.Settings;
using PVG.Domain.Utilities;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.MDataRepository;

namespace PVG.Application.Services.MDataService
{
    public class MDataService : BaseService, IMDataService
    {
        private readonly IMDataRepository _mDataRepository;

        public MDataService(IOptions<AppSettings> settings, IMapper mapper, IMDataRepository mDataRepository) : base(settings, mapper)
        {
            _mDataRepository = mDataRepository;
        }

        public async Task<BaseResponse> CreateMdata(List<MDataModel> _payload, string _user)
        {
            try
            {
                var groupList = _payload.Select(c => c.Group).Distinct().ToList();
                var existingMData = await _mDataRepository.FindByCondition(c => groupList.Contains(c.Group)).ToListAsync();
                if (existingMData.Any())
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "MData already exists for one or more groups.");

                var mdataEntity = _mapper.Map<List<MData>>(_payload);
                mdataEntity.ForEach(c => { c.CreatedByName = _user; c.CreatedDate = DateTime.Now; c.GroupName = c.Group.GetDisplayName(); });
                await _mDataRepository.CreateListAsync(mdataEntity);

                return SuccessResponse(true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"CreateMdata - {ex.Message}");
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, "Lỗi hệ thống.");
            }
        }

        public async Task<BaseResponse> UpdateMdata(int _id, MDataModel _payload, string _user)
        {
            try
            {
                var existingMData = await _mDataRepository.GetByIdAsync(_id);
                if (existingMData == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "MData not found.");

                _mapper.Map(_payload, existingMData);
                existingMData.ModifiedByName = _user;
                existingMData.ModifiedDate = DateTime.UtcNow;
                await _mDataRepository.UpdateAsync(existingMData);

                return SuccessResponse(true, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"UpdateMdata - {ex.Message}");
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, "Lỗi hệ thống.");
            }
        }

        public async Task<BaseResponse> RemoveMData(int _id, string _user)
        {
            try
            {
                var existingMData = await _mDataRepository.GetByIdAsync(_id);
                if (existingMData == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_REQUEST_NOT_FOUND, "MData not found.");

                existingMData.IsDeleted = true;
                existingMData.ModifiedByName = _user;
                existingMData.ModifiedDate = DateTime.UtcNow;
                await _mDataRepository.UpdateAsync(existingMData);

                return SuccessResponse(true, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"RemoveMData - {ex.Message}");
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, "Lỗi hệ thống.");
            }
        }

        public async Task<BaseResponse> GetMDataByGroupAsync(List<MDataEnum_Group> _group)
        {
            try
            {
                if (_group == null || _group.Count == 0)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Vui lòng chọn ít nhất 1 group.");

                var mdataEntities = await _mDataRepository.FindByCondition(c => _group.Contains(c.Group)).ToListAsync();
                var mdataModels = _mapper.Map<List<MDataResponseModel>>(mdataEntities);
                return SuccessResponse(mdataModels);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"GetMDataByGroupAsync - {ex.Message}");
                return BadRequestResponse(ErrorCodeConst.ERROR_SYS_ERR, "Lỗi hệ thống.");
            }
        }
    }
}