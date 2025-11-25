using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ProductDetailCategoryRepository
{
    public class ProductDetailCategoryRepository : RepositoryBase<ProductDetailCategory, Guid>, IProductDetailCategoryRepository
    {
        protected readonly PVGDbContext dbContext;
        public ProductDetailCategoryRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }
    }
}
