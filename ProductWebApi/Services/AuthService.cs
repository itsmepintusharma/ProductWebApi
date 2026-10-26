using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductWebApi.Data;
using ProductWebApi.DTO;
using ProductWebApi.Models;

namespace ProductWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser>
            _userManager;

        private readonly RoleManager<IdentityRole>
            _roleManager;

        private readonly IJwtService
            _jwtService;

        private readonly IRefreshTokenService
            _refreshTokenService;

        private readonly AppDbContext
            _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _context = context;
        }

        public async Task<string>
            RegisterAsync(
            RegisterRequest request)
        {
            var roleExists =
                await _roleManager
                    .RoleExistsAsync(
                        request.Role);

            if (!roleExists)
            {
                throw new Exception(
                    "Role does not exist");
            }

            var user =
                new ApplicationUser
                {
                    FullName =
                        request.FullName,

                    Email =
                        request.Email,

                    UserName =
                        request.Email
                };

            var result =
                await _userManager
                    .CreateAsync(
                        user,
                        request.Password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(",",
                    result.Errors
                        .Select(x =>
                            x.Description)));
            }

            await _userManager
                .AddToRoleAsync(
                    user,
                    request.Role);

            return "User Registered Successfully";
        }

        public async Task<LoginResponse>
            LoginAsync(
            LoginRequest request)
        {
            var user =
                await _userManager
                    .FindByEmailAsync(
                        request.Email);

            if (user == null)
                throw new UnauthorizedAccessException();

            var validPassword =
                await _userManager
                    .CheckPasswordAsync(
                        user,
                        request.Password);

            if (!validPassword)
                throw new UnauthorizedAccessException();

            var roles =
                await _userManager
                    .GetRolesAsync(user);

            var accessToken =
                _jwtService
                    .GenerateAccessToken(
                        user,
                        roles);

            var refreshToken =
                await _refreshTokenService
                    .CreateAsync(user);

            return new LoginResponse
            {
                AccessToken =
                    accessToken,

                RefreshToken =
                    refreshToken.Token
            };
        }

        public async Task<LoginResponse>
            RefreshAsync(
            string refreshToken)
        {
            var token =
                await _context.RefreshTokens
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(
                        x => x.Token ==
                             refreshToken);

            if (token == null)
                throw new Exception(
                    "Invalid Refresh Token");

            if (token.IsRevoked)
                throw new Exception(
                    "Refresh Token Revoked");

            if (token.ExpiresAt <
               DateTime.UtcNow)
            {
                throw new Exception(
                    "Refresh Token Expired");
            }

            var roles =
                await _userManager
                    .GetRolesAsync(
                        token.User);

            var accessToken =
                _jwtService
                    .GenerateAccessToken(
                        token.User,
                        roles);

            return new LoginResponse
            {
                AccessToken =
                    accessToken,

                RefreshToken =
                    token.Token
            };
        }
    }
}
