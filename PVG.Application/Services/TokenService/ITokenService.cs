using PVG.Infrastucture.Entities;

namespace PVG.Application.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user, TimeSpan? lifetime = null);
    }
}