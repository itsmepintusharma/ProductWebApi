using ProductWebApi.DTO;

namespace ProductWebApi.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>>
            GetAllAsync();

        Task<ProductResponseDto?>
            GetByIdAsync(int id);

        Task<ProductResponseDto>
            CreateAsync(ProductCreateDto dto);

        Task UpdateAsync(
            int id,
            ProductUpdateDto dto);

        Task DeleteAsync(int id);
    }
}
