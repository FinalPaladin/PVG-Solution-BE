using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.SampleRepository
{
    public class SampleRepository : RepositoryBase<Sample, Guid>, ISampleRepository
    {
        public SampleRepository(PVGDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}