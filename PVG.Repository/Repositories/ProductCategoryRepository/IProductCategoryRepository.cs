using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ProductCategoryRepository
{
    public interface IProductCategoryRepository : IRepositoryBase<ProductCategory, Guid>
    {
        public Task<ProductCategory> GetById(Guid _id);
    }
}
