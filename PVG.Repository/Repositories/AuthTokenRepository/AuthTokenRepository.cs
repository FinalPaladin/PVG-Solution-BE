using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Persistence;

namespace PVG.Infrastucture.Repositories.AuthTokenRepository
{
    public class AuthTokenRepository : RepositoryBase<AuthToken, Guid>, IAuthTokenRepository
    {
        public AuthTokenRepository(PVGDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}