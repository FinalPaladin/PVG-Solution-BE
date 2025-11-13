using Microsoft.EntityFrameworkCore;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.UserRepository
{
    public class UserRepository : RepositoryBase<User, Guid>, IUserRepository
    {
        protected readonly PVGDbContext dbContext;
        public UserRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

        public async Task<User> GetById(Guid _id)
        {
            return await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id.Equals(_id));
        }
    }
}
