using Microsoft.EntityFrameworkCore;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ProductInfoRepository
{
    public class ProductInfoRepository : RepositoryBase<ProductInfo, Guid>, IProductInfoRepository
    {
        protected readonly PVGDbContext dbContext;
        public ProductInfoRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

        public async Task<ProductInfo> GetById(Guid _id)
        {
            return await dbContext.Set<ProductInfo>().FirstOrDefaultAsync(x => x.Id.Equals(_id));
        }
    }
}
