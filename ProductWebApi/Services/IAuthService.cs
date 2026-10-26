using ProductWebApi.DTO;

namespace ProductWebApi.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(
            RegisterRequest request);

        Task<LoginResponse> LoginAsync(
            LoginRequest request);

        Task<LoginResponse> RefreshAsync(
            string refreshToken);
    }
}
