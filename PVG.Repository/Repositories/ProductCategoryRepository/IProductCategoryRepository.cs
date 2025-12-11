using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.ProductCategoryRepository
{
    public interface IProductCategoryRepository : IRepositoryBase<ProductCategory, Guid>
    {
    }
}