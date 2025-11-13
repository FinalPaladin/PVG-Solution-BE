using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ConfigurationRepository
{
    public interface IConfigurationRepository : IRepositoryBase<Configuration, Guid>
    {
        public Task<Configuration> GetById(Guid _id);
    }
}
