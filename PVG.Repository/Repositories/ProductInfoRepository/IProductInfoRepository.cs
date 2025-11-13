using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.ProductInfoRepository
{
    public interface IProductInfoRepository : IRepositoryBase<ProductInfo, Guid>
    {
        public Task<ProductInfo> GetById(Guid _id);
    }
}
