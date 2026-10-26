using ProductWebApi.Models;

namespace ProductWebApi.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(
            ApplicationUser user,
            IList<string> roles);

        string GenerateRefreshToken();
    }
}
