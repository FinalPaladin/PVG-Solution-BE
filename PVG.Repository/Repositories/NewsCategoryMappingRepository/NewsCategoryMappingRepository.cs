using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.NewsCategoryMappingRepository
{
    public class NewsCategoryMappingRepository : RepositoryBase<NewsCategoryMapping, Guid>, INewsCategoryMappingRepository
    {
        public NewsCategoryMappingRepository(PVGDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}