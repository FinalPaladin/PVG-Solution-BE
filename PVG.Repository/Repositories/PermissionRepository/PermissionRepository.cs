using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.PermissionRepository
{
    public class PermissionRepository : RepositoryBase<Permission, int>, IPermissionRepository
    {
        protected readonly PVGDbContext dbContext;

        public PermissionRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }
    }
}