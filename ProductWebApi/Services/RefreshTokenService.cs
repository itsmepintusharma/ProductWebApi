using Microsoft.EntityFrameworkCore;
using ProductWebApi.Data;
using ProductWebApi.Models;

namespace ProductWebApi.Services
{
    public class RefreshTokenService
    : IRefreshTokenService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public RefreshTokenService(
            AppDbContext context,
            IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<RefreshToken>
            CreateAsync(ApplicationUser user)
        {
            var refreshToken =
                new RefreshToken
                {
                    UserId = user.Id,
                    Token =
                        _jwtService.GenerateRefreshToken(),

                    CreatedAt = DateTime.UtcNow,

                    ExpiresAt =
                        DateTime.UtcNow.AddDays(7),

                    IsRevoked = false
                };

            _context.RefreshTokens.Add(
                refreshToken);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<bool>
            RevokeAsync(string token)
        {
            var entity =
                await _context.RefreshTokens
                    .FirstOrDefaultAsync(
                        x => x.Token == token);

            if (entity == null)
                return false;

            entity.IsRevoked = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
