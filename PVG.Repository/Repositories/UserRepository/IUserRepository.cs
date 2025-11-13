using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.UserRepository
{
    public interface IUserRepository : IRepositoryBase<User, Guid>
    {
        public Task<User> GetById(Guid _id);
    }
}
