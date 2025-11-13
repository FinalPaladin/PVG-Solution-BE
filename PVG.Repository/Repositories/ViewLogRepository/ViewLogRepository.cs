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

namespace PVG.Infrastucture.Repositories.ViewLogRepository
{
    public class ViewLogRepository : RepositoryBase<ViewLog, Guid>, IViewLogRepository
    {
        protected readonly PVGDbContext dbContext;
        public ViewLogRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

        public async Task<ViewLog> GetById(Guid _id)
        {
            return await dbContext.Set<ViewLog>().FirstOrDefaultAsync(x => x.Id.Equals(_id));
        }
    }
}
