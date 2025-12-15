using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.NewsRepository
{
    public class NewsRepository : RepositoryBase<News, Guid>, INewsRepository
    {
        public NewsRepository(PVGDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}