using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Repositories.NewsCategoryMappingRepository
{
    public class NewsCategoryMappingRepository : RepositoryBase<NewsCategoryMapping, Guid>, INewsCategoryMappingRepository
    {
        public NewsCategoryMappingRepository(PVGDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}