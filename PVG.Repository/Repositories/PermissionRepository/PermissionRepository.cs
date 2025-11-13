using Microsoft.EntityFrameworkCore;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.PermissionRepository
{
    public class PermissionRepository : RepositoryBase<Permission, Guid>, IPermissionRepository
    {
        protected readonly PVGDbContext dbContext;
        public PermissionRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

        public async Task<Permission> GetById(Guid _id)
        {
            return await dbContext.Set<Permission>().FirstOrDefaultAsync(x => x.Id.Equals(_id));
        }
    }
}
