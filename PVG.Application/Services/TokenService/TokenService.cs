using AutoMapper;
using Microsoft.Extensions.Options;
using PVG.Domain.Settings;
using PVG.Infrastucture.Entities;
using PVG.Infrastucture.Repositories.AuthTokenRepository;
using System.Security.Cryptography;

namespace PVG.Application.Services.TokenService
{
    public class TokenService : BaseService, ITokenService
    {
        private readonly IAuthTokenRepository _authTokenRepository;

        public TokenService(
            IOptions<AppSettings> options,
            IMapper mapper, 
            IAuthTokenRepository authTokenRepository
            ) 
            : base(options, mapper)
        {
            _authTokenRepository = authTokenRepository;
        }

        public async Task<string> CreateTokenAsync(User user, TimeSpan? lifetime = null)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(32); // 256-bit
            var token = Convert.ToBase64String(tokenBytes);

            var authToken = new AuthToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            };
            await _authTokenRepository.CreateAsync(authToken);

            return token;
        }
    }
}