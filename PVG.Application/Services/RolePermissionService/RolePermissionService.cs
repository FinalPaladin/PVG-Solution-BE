using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Extensions;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.PermissionRepository;
using PVG.Infrastucture.Repositories.RolePermissionRepository;
using PVG.Infrastucture.Repositories.RoleRepository;

namespace PVG.Application.Services.RolePermissionService
{
    public class RolePermissionService : BaseService, IRolePermissionService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionService(
            IOptions<AppSettings> settings,
            IMapper mapper,

            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRolePermissionRepository rolePermissionRepository
            ) : base(settings, mapper)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<BaseResponse> CreateRole(RoleCreatingModel model, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Tên role không được để trống");

                var roleExist = await _roleRepository
                    .FindByCondition(r => r.Name == model.Name && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (roleExist != null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Tên role đã tồn tại");

                var role = _mapper.Map<Role>(model);
                role.CreatedByName = userName;
                role.CreatedDate = DateTime.Now;

                using var transaction = await _roleRepository.BeginTransactionAsync();
                try
                {
                    var roleId = await _roleRepository.CreateAsync(role);

                    var permissions = await _permissionRepository
                        .FindByCondition(p => true)
                        .ToListAsync();

                    var rolePermissions = permissions.Select(p => new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = p.Id
                    }).ToList();

                    await _rolePermissionRepository.CreateListAsync(rolePermissions);

                    transaction.Commit();
                    return SuccessResponse(new { RoleId = roleId }, "success");
                }
                catch (Exception ex)
                {
                    await _roleRepository.RollbackTransactionAsync();
                    Logger.Error(ex);
                    return CatchErrorResponse(ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> UpdateRole(int roleId, RoleUpdatingModel model, string userName)
        {
            try
            {
                var role = await _roleRepository
                    .FindByCondition(r => r.Id == roleId && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (role == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_DATA_NOT_FOUND, "Role không tồn tại");

                var duplicate = await _roleRepository
                    .FindByCondition(r => r.Name == model.Name && r.Id != roleId && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (duplicate != null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_INPUT_INVALID, "Tên role đã tồn tại");

                role.Name = model.Name;
                role.Description = model.Description;
                role.Inactive = model.Inactive;
                role.ModifiedByName = userName;
                role.ModifiedDate = DateTime.Now;

                await _roleRepository.UpdateAsync(role);

                return SuccessResponse(null, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> DeleteRole(int roleId, string userName)
        {
            try
            {
                var role = await _roleRepository
                    .FindByCondition(r => r.Id == roleId && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (role == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_DATA_NOT_FOUND, "Role không tồn tại");

                role.IsDeleted = true;
                role.DeletedByName = userName;
                role.DeletedDate = DateTime.Now;

                await _roleRepository.UpdateAsync(role);

                return SuccessResponse(null, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetRoles(RoleQueryModel query)
        {
            try
            {
                var rolesQuery = _roleRepository
                    .FindByCondition(r => !r.IsDeleted);

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    rolesQuery = rolesQuery.Where(r =>
                        r.Name.Contains(query.Keyword));
                }

                var roles = await rolesQuery
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                return SuccessResponse(roles, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> UpdateRolePermissions(
            int roleId,
            List<RolePermissionUpdatingModel> permissions,
            string userName)
        {
            try
            {
                var role = await _roleRepository
                    .FindByCondition(r => r.Id == roleId && !r.IsDeleted)
                    .FirstOrDefaultAsync();

                if (role == null)
                    return BadRequestResponse(ErrorCodeConst.ERROR_DATA_NOT_FOUND, "Role không tồn tại");

                var rolePermissions = await _rolePermissionRepository
                    .FindByCondition(rp => rp.RoleId == roleId)
                    .ToListAsync();

                foreach (var item in permissions)
                {
                    var rp = rolePermissions
                        .FirstOrDefault(x => x.PermissionId == item.PermissionId);

                    if (rp == null) continue;

                    rp.CanView = item.CanView;
                    rp.CanCreate = item.CanCreate;
                    rp.CanEdit = item.CanEdit;
                    rp.CanDelete = item.CanDelete;
                    rp.CanReport = item.CanReport;
                    rp.CanApprove = item.CanApprove;
                }

                await _rolePermissionRepository.UpdateListAsync(rolePermissions);

                return SuccessResponse(null, "success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return CatchErrorResponse(ex);
            }
        }
    }
}