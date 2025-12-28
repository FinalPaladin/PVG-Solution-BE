using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.ProductDetailRepository
{
    public class ProductDetailRepository : RepositoryBase<ProductDetail, Guid>, IProductDetailRepository
    {
        protected readonly PVGDbContext dbContext;

        public ProductDetailRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }
    }
}