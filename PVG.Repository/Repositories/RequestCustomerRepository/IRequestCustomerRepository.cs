using PVG.Domain.Models;
using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Infrastucture.Repositories.RequestCustomerRepository
{
    public interface IRequestCustomerRepository : IRepositoryBase<RequestCustomer, Guid>
    {
    }
}
