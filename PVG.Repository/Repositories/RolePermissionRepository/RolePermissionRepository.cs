using PVG.Infrastucture.Domain;

namespace PVG.Infrastucture.Repositories.RolePermissionRepository
{
    public class RolePermissionRepository : RepositoryBase<Entities.RolePermission, int>, IRolePermissionRepository
    {
        public RolePermissionRepository(PVGDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}