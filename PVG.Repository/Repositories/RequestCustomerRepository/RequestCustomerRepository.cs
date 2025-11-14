using Microsoft.EntityFrameworkCore;
using PVG.Domain.Models;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;
using PVG.Infrastucture.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.RequestCustomerRepository
{
    public class RequestCustomerRepository : RepositoryBase<RequestCustomer, Guid>, IRequestCustomerRepository
    {
        private readonly PVGDbContext dbContext;
        public RequestCustomerRepository(PVGDbContext _dbContext, IUnitOfWork _unitOfWork) : base(_dbContext, _unitOfWork)
        {
            dbContext = _dbContext;
        }

    }
}
