using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Repositories.MDataRepository
{
    public class MDataRepository : RepositoryBase<MData, int>, IMDataRepository
    {
        public MDataRepository(PVGDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}