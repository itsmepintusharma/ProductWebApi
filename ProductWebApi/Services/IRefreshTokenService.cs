using ProductWebApi.Models;

namespace ProductWebApi.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateAsync(
            ApplicationUser user);

        Task<bool> RevokeAsync(
            string token);
    }
}
