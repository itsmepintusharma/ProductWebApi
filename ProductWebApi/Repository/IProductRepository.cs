using ProductWebApi.Models;

namespace ProductWebApi.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product product);

        Task UpdateAsync(Product product);

        Task DeleteAsync(Product product);
    }
}
