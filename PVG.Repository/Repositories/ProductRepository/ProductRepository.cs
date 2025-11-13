using Microsoft.EntityFrameworkCore;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ProductRepository
{
    public class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
    {
        protected readonly PVGDbContext dbContext;
        public ProductRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

        public async Task<Product> GetById(Guid _id)
        {
            return await dbContext.Set<Product>().FirstOrDefaultAsync(x => x.Id.Equals(_id));
        }
    }
}
