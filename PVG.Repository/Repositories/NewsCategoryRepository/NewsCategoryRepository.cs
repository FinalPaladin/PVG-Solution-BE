using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Repositories.NewsCategoryRepository
{
    public class NewsCategoryRepository : RepositoryBase<NewsCategory, Guid>, INewsCategoryRepository
    {
        public NewsCategoryRepository(PVGDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}