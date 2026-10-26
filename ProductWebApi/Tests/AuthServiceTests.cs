using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ProductWebApi.Data;
using ProductWebApi.DTO;
using ProductWebApi.Models;
using ProductWebApi.Services;


namespace ProductWebApi.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<UserManager<ApplicationUser>>
        _userManagerMock = null!;

        private Mock<RoleManager<IdentityRole>>
        _roleManagerMock = null!;

        private Mock<IJwtService>
        _jwtServiceMock = null!;

        private Mock<IRefreshTokenService>
        _refreshTokenServiceMock = null!;

        private AppDbContext _context = null!;

        private AuthService _authService = null!;

        [SetUp]
        public void Setup()
        {
            var userStore =
            new Mock<IUserStore<ApplicationUser>>();

            _userManagerMock =
            new Mock<UserManager<ApplicationUser>>
            (
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
            );

            var roleStore =
            new Mock<IRoleStore<IdentityRole>>();

            _roleManagerMock =
            new Mock<RoleManager<IdentityRole>>
            (
            roleStore.Object,
            null!,
            null!,
            null!,
            null!
            );

            _jwtServiceMock =
            new Mock<IJwtService>();

            _refreshTokenServiceMock =
            new Mock<IRefreshTokenService>();

            var options =
            new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(
            Guid.NewGuid().ToString())
            .Options;

            _context =
            new AppDbContext(options);

            _authService =
            new AuthService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _jwtServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _context);
        }
        [Test]
        public void Register_InvalidRole_Should_Throw()
        {
            var request =
                new RegisterRequest
                {
                    FullName = "John",
                    Email = "john@test.com",
                    Password = "Password@123",
                    Role = "SuperAdmin"
                };

            _roleManagerMock
                .Setup(x =>
                    x.RoleExistsAsync(
                        "SuperAdmin"))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<Exception>(
                async () =>
                {
                    await _authService
                        .RegisterAsync(request);
                });
        }

        [Test]
        public async Task Login_Should_Return_Token()
        {
            var user =
                new ApplicationUser
                {
                    Id = "1",
                    Email = "admin@test.com",
                    UserName = "admin@test.com"
                };

            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        user.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x =>
                    x.CheckPasswordAsync(
                        user,
                        "Admin@123"))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x =>
                    x.GetRolesAsync(user))
                .ReturnsAsync(
                    new List<string>
                    {
                "Admin"
                    });

            _jwtServiceMock
                .Setup(x =>
                    x.GenerateAccessToken(
                        user,
                        It.IsAny<IList<string>>()))
                .Returns("jwt-token");

            _refreshTokenServiceMock
                .Setup(x =>
                    x.CreateAsync(user))
                .ReturnsAsync(
                    new RefreshToken
                    {
                        Token = "refresh-token"
                    });

            var response =
                await _authService
                    .LoginAsync(
                        new LoginRequest
                        {
                            Email = user.Email,
                            Password = "Admin@123"
                        });

            response.Should().NotBeNull();

            response.AccessToken
                .Should()
                .Be("jwt-token");
        }

        [Test]
        public void Login_UserNotFound_Should_Throw()
        {
            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        It.IsAny<string>()))
                .ReturnsAsync(
                    (ApplicationUser?)null);

            Assert.ThrowsAsync
                <UnauthorizedAccessException>(
                async () =>
                {
                    await _authService
                        .LoginAsync(
                            new LoginRequest
                            {
                                Email = "fake@test.com",
                                Password = "123"
                            });
                });
        }

        [Test]
        public void Login_InvalidPassword_Should_Throw()
        {
            var user =
                new ApplicationUser
                {
                    Email = "admin@test.com"
                };

            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        user.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x =>
                    x.CheckPasswordAsync(
                        user,
                        "wrong"))
                .ReturnsAsync(false);

            Assert.ThrowsAsync
                <UnauthorizedAccessException>(
                async () =>
                {
                    await _authService
                        .LoginAsync(
                            new LoginRequest
                            {
                                Email = user.Email,
                                Password = "wrong"
                            });
                });
        }
        [Test]
        public async Task RefreshToken_Should_Return_NewToken()
        {
            var user =
                new ApplicationUser
                {
                    Id = "1",
                    Email = "admin@test.com"
                };

            var token =
                new RefreshToken
                {
                    Token = "refresh123",
                    User = user,
                    ExpiresAt =
                        DateTime.UtcNow.AddDays(1)
                };

            _context.RefreshTokens.Add(token);

            await _context.SaveChangesAsync();

            _userManagerMock
                .Setup(x =>
                    x.GetRolesAsync(user))
                .ReturnsAsync(
                    new List<string>
                    {
                "Admin"
                    });

            _jwtServiceMock
                .Setup(x =>
                    x.GenerateAccessToken(
                        user,
                        It.IsAny<IList<string>>()))
                .Returns("new-token");

            var result =
                await _authService
                    .RefreshAsync("refresh123");

            result.AccessToken
                .Should()
                .Be("new-token");
        }
        [Test]
        public void RefreshToken_Invalid_Should_Throw()
        {
            Assert.ThrowsAsync<Exception>(
                async () =>
                {
                    await _authService
                        .RefreshAsync(
                            "invalid");
                });
        }

        [Test]
        public void RefreshToken_Expired_Should_Throw()
        {
            var user =
                new ApplicationUser
                {
                    Id = "1"
                };

            _context.RefreshTokens.Add(
                new RefreshToken
                {
                    Token = "expired",

                    User = user,

                    ExpiresAt =
                        DateTime.UtcNow.AddDays(-1)
                });

            _context.SaveChanges();

            Assert.ThrowsAsync<Exception>(
                async () =>
                {
                    await _authService
                        .RefreshAsync(
                            "expired");
                });
        }
    }

}
