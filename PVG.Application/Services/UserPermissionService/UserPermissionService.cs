using PVG.Infrastucture.Repositories.UserPermissionRepository;

namespace PVG.Application.Services.UserPermissionService
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository _userPermissionRepository;

        public UserPermissionService(IUserPermissionRepository userPermissionRepository)
        {
            _userPermissionRepository = userPermissionRepository;
        }
    }
}