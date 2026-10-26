using FluentAssertions;
using NUnit.Framework;
using ProductWebApi.Models;
using ProductWebApi.Services;

namespace ProductWebApi.Tests
{
    [TestFixture]
    public class JwtServiceTests
    {
        private JwtService _jwtService = null!;

        [SetUp]
        public void Setup()
        {
            var configuration =
                new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                    {
                        "Jwt:Key",
                        "THIS_IS_MY_SUPER_SECRET_KEY_256_BIT_2026"
                    },
                    {
                        "Jwt:Issuer",
                        "SecureCrudApi"
                    },
                    {
                        "Jwt:Audience",
                        "SecureCrudUsers"
                    }
                    })
                .Build();

            _jwtService =
                new JwtService(configuration);
        }

        [Test]
        public void
            GenerateRefreshToken_Should_Not_Return_Null()
        {
            var token =
                _jwtService.GenerateRefreshToken();

            token.Should()
                .NotBeNull();

            token.Should()
                .NotBeEmpty();
        }

        [Test]
        public void
            GenerateRefreshToken_Should_Return_Unique_Tokens()
        {
            var token1 =
                _jwtService.GenerateRefreshToken();

            var token2 =
                _jwtService.GenerateRefreshToken();

            token1.Should()
                .NotBe(token2);
        }

        [Test]
        public void
            GenerateAccessToken_Should_Return_Valid_Token()
        {
            var user =
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };

            var roles =
                new List<string>
                {
                "Admin"
                };

            var token =
                _jwtService.GenerateAccessToken(
                    user,
                    roles);

            token.Should()
                .NotBeNull();

            token.Should()
                .NotBeEmpty();
        }

        [Test]
        public void
            GenerateAccessToken_Should_Return_Jwt_Format()
        {
            var user =
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };

            var roles =
                new List<string>
                {
                "Admin"
                };

            var token =
                _jwtService.GenerateAccessToken(
                    user,
                    roles);

            token.Split('.')
                 .Length
                 .Should()
                 .Be(3);
        }

        [Test]
        public void
            GenerateAccessToken_For_User_Role()
        {
            var user =
                new ApplicationUser
                {
                    Id = "2",
                    UserName = "user@test.com",
                    Email = "user@test.com"
                };

            var roles =
                new List<string>
                {
                "User"
                };

            var token =
                _jwtService.GenerateAccessToken(
                    user,
                    roles);

            token.Should()
                 .NotBeNullOrWhiteSpace();
        }

        [Test]
        public void
            GenerateAccessToken_For_Admin_Role()
        {
            var user =
                new ApplicationUser
                {
                    Id = "3",
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };

            var roles =
                new List<string>
                {
                "Admin"
                };

            var token =
                _jwtService.GenerateAccessToken(
                    user,
                    roles);

            token.Should()
                 .NotBeNullOrWhiteSpace();
        }
    }
}