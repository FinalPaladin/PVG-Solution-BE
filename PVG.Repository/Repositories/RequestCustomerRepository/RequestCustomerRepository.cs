using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task EditAsync(RequestCustomer entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyValue = entity.RequestCode; // PK MỚI

            var exist = await dbContext.Set<RequestCustomer>()
                .SingleOrDefaultAsync(x => x.RequestCode == keyValue);

            if (exist == null)
                throw new Exception($"Không tìm thấy entity có RequestCode = {keyValue}");

            dbContext.Entry(exist).CurrentValues.SetValues(entity);
        }
    }
}
