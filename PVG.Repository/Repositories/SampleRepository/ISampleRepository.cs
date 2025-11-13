using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.SampleRepository
{
    public interface ISampleRepository : IRepositoryBase<Sample, Guid>
    {
    }
}