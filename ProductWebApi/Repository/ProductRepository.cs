using Microsoft.EntityFrameworkCore;
using ProductWebApi.Data;
using ProductWebApi.Models;

namespace ProductWebApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
        }
    }
}
