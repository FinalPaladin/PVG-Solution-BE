using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.NewsCategoryRepository
{
    public class NewsCategoryRepository : RepositoryBase<NewsCategory, Guid>, INewsCategoryRepository
    {
        public NewsCategoryRepository(PVGDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}