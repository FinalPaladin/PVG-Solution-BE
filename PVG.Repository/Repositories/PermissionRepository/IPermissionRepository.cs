using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.PermissionRepository
{
    public interface IPermissionRepository : IRepositoryBase<Permission, Guid>
    {
        public Task<Permission> GetById(Guid _id);
    }
}
