using PVG.Infrastucture.Domain;
using PVG.Infrastucture.Entities;

namespace PVG.Infrastucture.Repositories.AuthTokenRepository
{
    public interface IAuthTokenRepository : IRepositoryBase<AuthToken, Guid>
    {
    }
}