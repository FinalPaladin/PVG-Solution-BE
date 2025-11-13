using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.UserPermissionRepository
{
    public interface IUserPermissionRepository : IRepositoryBase<UserPermission, Guid>
    {
        public Task<UserPermission> GetById(Guid _id);
    }
}
