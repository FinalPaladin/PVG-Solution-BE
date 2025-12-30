using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.ProductCategoryRepository
{
    public class ProductCategoryRepository : RepositoryBase<ProductCategory, Guid>, IProductCategoryRepository
    {
        protected readonly PVGDbContext dbContext;

        public ProductCategoryRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }
    }
}