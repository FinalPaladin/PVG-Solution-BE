using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Repositories.SampleReporitory
{
    public class SampleRepository : RepositoryBase<Sample, int>, ISampleRepository
    {
        public SampleRepository(PVGDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}