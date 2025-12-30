using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.ProductDetailRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.RequestCustomerDetailRepository
{
    public class RequestCustomerDetailRepository : RepositoryBase<RequestCustomerDetail, Guid>, IRequestCustomerDetailRepository
    {
        protected readonly PVGDbContext dbContext;
        public RequestCustomerDetailRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }
    }
}
