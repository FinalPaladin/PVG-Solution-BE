using PVG.Infrastucture.Entities;
using System.Security.Cryptography;

namespace PVG.Application.Services.TokenService
{
    public class TokenService : BaseService, ITokenService
    {
        public TokenService()
        { }

        public async Task<string> CreateTokenAsync(User user, TimeSpan? lifetime = null)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(32); // 256-bit
            var token = Convert.ToBase64String(tokenBytes);

            return token;
        }
    }
}