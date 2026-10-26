using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProductWebApi.Data;
using ProductWebApi.DTO;
using ProductWebApi.Models;
using ProductWebApi.Repository;
using ProductWebApi.Services;

namespace ProductWebApi.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _repositoryMock = null!;

        private ProductService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock =
                new Mock<IProductRepository>();

            _service =
                new ProductService(
                    _repositoryMock.Object);
        }

        [Test]
        public async Task Create_Product_Should_Return_Product()
        {
            // Arrange

            var dto =
                new ProductCreateDto
                {
                    Name = "Laptop",
                    Description = "Dell Laptop",
                    Price = 50000,
                    Stock = 10
                };

            _repositoryMock
                .Setup(x =>
                    x.CreateAsync(
                        It.IsAny<Product>()))
                .ReturnsAsync(
                    (Product p) =>
                    {
                        p.Id = 1;
                        return p;
                    });

            // Act

            var result =
                await _service.CreateAsync(dto);

            // Assert

            result.Should().NotBeNull();

            result.Id.Should().Be(1);

            result.Name.Should().Be("Laptop");

            result.Stock.Should().Be(10);
        }

        [Test]
        public async Task GetAll_Should_Return_Products()
        {
            // Arrange

            var products =
                new List<Product>
                {
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "Dell",
                    Price = 50000,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Mouse",
                    Description = "Wireless",
                    Price = 1000,
                    Stock = 20
                }
                };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(products);

            // Act

            var result =
                await _service.GetAllAsync();

            // Assert

            result.Should().HaveCount(2);
        }

        [Test]
        public async Task GetById_Should_Return_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "Dell Laptop",
                    Price = 50000,
                    Stock = 10
                };

            _repositoryMock
                .Setup(x =>
                    x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act

            var result =
                await _service.GetByIdAsync(1);

            // Assert

            result.Should().NotBeNull();

            result!.Name.Should().Be("Laptop");
        }

        [Test]
        public async Task GetById_Should_Return_Null()
        {
            // Arrange

            _repositoryMock
                .Setup(x =>
                    x.GetByIdAsync(999))
                .ReturnsAsync(
                    (Product?)null);

            // Act

            var result =
                await _service.GetByIdAsync(999);

            // Assert

            result.Should().BeNull();
        }

        [Test]
        public async Task Update_Should_Update_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Id = 1,
                    Name = "Old Laptop",
                    Description = "Old Description",
                    Price = 40000,
                    Stock = 5
                };

            var dto =
                new ProductUpdateDto
                {
                    Name = "New Laptop",
                    Description = "Updated Description",
                    Price = 60000,
                    Stock = 15
                };

            _repositoryMock
                .Setup(x =>
                    x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act

            await _service.UpdateAsync(
                1,
                dto);

            // Assert

            _repositoryMock.Verify(
                x => x.UpdateAsync(
                    It.Is<Product>(
                        p =>
                            p.Name == "New Laptop" &&
                            p.Stock == 15)),
                Times.Once);
        }

        [Test]
        public void Update_Product_NotFound_Should_Throw()
        {
            // Arrange

            _repositoryMock
                .Setup(x =>
                    x.GetByIdAsync(1))
                .ReturnsAsync(
                    (Product?)null);

            var dto =
                new ProductUpdateDto
                {
                    Name = "Laptop",
                    Description = "Dell",
                    Price = 1000,
                    Stock = 1
                };

            // Assert

            Assert.ThrowsAsync<Exception>(
                async () =>
                {
                    await _service.UpdateAsync(
                        1,
                        dto);
                });
        }

        [Test]
        public async Task Delete_Should_Delete_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "Dell",
                    Price = 50000,
                    Stock = 10
                };

            _repositoryMock
                .Setup(x =>
                    x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act

            await _service.DeleteAsync(1);

            // Assert

            _repositoryMock.Verify(
                x => x.DeleteAsync(product),
                Times.Once);
        }

        [Test]
        public void Delete_Product_NotFound_Should_Throw()
        {
            // Arrange

            _repositoryMock
                .Setup(x =>
                    x.GetByIdAsync(1))
                .ReturnsAsync(
                    (Product?)null);

            // Assert

            Assert.ThrowsAsync<Exception>(
                async () =>
                {
                    await _service.DeleteAsync(1);
                });
        }
    }
}
