using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ViewLogRepository
{
    public interface IViewLogRepository : IRepositoryBase<ViewLog, Guid>
    {
        public Task<ViewLog> GetById(Guid _id);
    }
}
