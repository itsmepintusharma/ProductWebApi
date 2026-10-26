using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductWebApi.Data;
using ProductWebApi.Models;
using ProductWebApi.Repository;
using ProductWebApi.Services;

namespace ProductWebApi.Tests
{
    [TestFixture]
    public class ProductRepositoryTests
    {
        private AppDbContext _context = null!;

        private ProductRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options =
                new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

            _context =
                new AppDbContext(options);

            _repository =
                new ProductRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();

            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_Should_Add_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Name = "Laptop",
                    Description = "Dell Laptop",
                    Price = 50000,
                    Stock = 10
                };

            // Act

            var result =
                await _repository.CreateAsync(product);

            // Assert

            result.Should().NotBeNull();

            result.Id.Should().BeGreaterThan(0);

            var count =
                await _context.Products.CountAsync();

            count.Should().Be(1);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_All_Products()
        {
            // Arrange

            _context.Products.AddRange(
                new Product
                {
                    Name = "Laptop",
                    Description = "Dell",
                    Price = 50000,
                    Stock = 10
                },
                new Product
                {
                    Name = "Mouse",
                    Description = "Wireless",
                    Price = 1000,
                    Stock = 50
                });

            await _context.SaveChangesAsync();

            // Act

            var result =
                await _repository.GetAllAsync();

            // Assert

            result.Should().NotBeNull();

            result.Count.Should().Be(2);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Name = "Keyboard",
                    Description = "Mechanical",
                    Price = 3000,
                    Stock = 20
                };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // Act

            var result =
                await _repository.GetByIdAsync(
                    product.Id);

            // Assert

            result.Should().NotBeNull();

            result!.Name.Should().Be("Keyboard");
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null()
        {
            // Act

            var result =
                await _repository.GetByIdAsync(999);

            // Assert

            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_Should_Update_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Name = "Laptop",
                    Description = "Dell",
                    Price = 50000,
                    Stock = 10
                };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // Act

            product.Name = "Updated Laptop";

            product.Stock = 20;

            await _repository.UpdateAsync(product);

            // Assert

            var updatedProduct =
                await _context.Products
                    .FirstAsync();

            updatedProduct.Name
                .Should()
                .Be("Updated Laptop");

            updatedProduct.Stock
                .Should()
                .Be(20);
        }

        [Test]
        public async Task DeleteAsync_Should_Remove_Product()
        {
            // Arrange

            var product =
                new Product
                {
                    Name = "Monitor",
                    Description = "LG Monitor",
                    Price = 12000,
                    Stock = 5
                };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // Act

            await _repository.DeleteAsync(product);

            // Assert

            var count =
                await _context.Products.CountAsync();

            count.Should().Be(0);
        }

        [Test]
        public async Task DeleteAsync_Product_Should_Not_Exist()
        {
            // Arrange

            var product =
                new Product
                {
                    Name = "Monitor",
                    Description = "LG Monitor",
                    Price = 12000,
                    Stock = 5
                };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // Act

            await _repository.DeleteAsync(product);

            // Assert

            var deletedProduct =
                await _repository.GetByIdAsync(
                    product.Id);

            deletedProduct
                .Should()
                .BeNull();
        }
    }
}
