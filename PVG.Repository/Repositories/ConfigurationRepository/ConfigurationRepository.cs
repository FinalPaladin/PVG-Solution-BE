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

namespace PVG.Infrastucture.Repositories.ConfigurationRepository
{
    public class ConfigurationRepository : RepositoryBase<Configuration, Guid>, IConfigurationRepository
    {
        protected readonly PVGDbContext dbContext;
        public ConfigurationRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

        public async Task<Configuration> GetById(Guid _id)
        {
            return await dbContext.Set<Configuration>().FirstOrDefaultAsync(x => x.Id.Equals(_id));
        }
    }
}
