using Microsoft.AspNetCore.Mvc;
using ProductWebApi.DTO;
using ProductWebApi.Services;

namespace ProductWebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController
    : ControllerBase
    {
        private readonly IAuthService
            _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult>
            Register(
            RegisterRequest request)
        {
            var result =
                await _authService
                    .RegisterAsync(request);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult>
            Login(
            LoginRequest request)
        {
            var result =
                await _authService
                    .LoginAsync(request);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult>
            RefreshToken(
            RefreshTokenRequest request)
        {
            var result =
                await _authService
                    .RefreshAsync(
                        request.RefreshToken);

            return Ok(result);
        }
    }
}
