using PVG.Core.BaseModels;
using PVG.Domain.Enums;
using PVG.Domain.Models;

namespace PVG.Application.Services.MDataService
{
    public interface IMDataService
    {
        Task<BaseResponse> CreateMdata(List<MDataModel> _payload, string _user);
        Task<BaseResponse> UpdateMdata(int _id, MDataModel _payload, string _user);
        Task<BaseResponse> RemoveMData(int _id, string _user);
        
        Task<BaseResponse> GetMDataByGroupAsync(List<MDataEnum_Group> _group);
    }
}