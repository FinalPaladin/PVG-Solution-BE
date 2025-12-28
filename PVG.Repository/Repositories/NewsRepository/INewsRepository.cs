using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.NewsRepository
{
    public interface INewsRepository : IRepositoryBase<News, Guid>
    {
    }
}