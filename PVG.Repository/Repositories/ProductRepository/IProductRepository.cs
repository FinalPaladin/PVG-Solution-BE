using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ProductRepository
{
    public interface IProductRepository : IRepositoryBase<Product, Guid>
    {
        public Task<Product> GetById(Guid _id);
    }
}
