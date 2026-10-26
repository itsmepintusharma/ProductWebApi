using ProductWebApi.DTO;
using ProductWebApi.Models;
using ProductWebApi.Repository;

namespace ProductWebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(
            IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductResponseDto>>
            GetAllAsync()
        {
            var products =
                await _repository.GetAllAsync();

            return products.Select(x =>
                new ProductResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Stock = x.Stock,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }

        public async Task<ProductResponseDto?>
            GetByIdAsync(int id)
        {
            var product =
                await _repository.GetByIdAsync(id);

            if (product == null)
                return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponseDto>
            CreateAsync(ProductCreateDto dto)
        {
            var product =
                new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    CreatedAt = DateTime.UtcNow
                };

            await _repository.CreateAsync(product);

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task UpdateAsync(
            int id,
            ProductUpdateDto dto)
        {
            var product =
                await _repository.GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception(
                    "Product Not Found");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

            await _repository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product =
                await _repository.GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception(
                    "Product Not Found");
            }

            await _repository.DeleteAsync(product);
        }
    }
}
