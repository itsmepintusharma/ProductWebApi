using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductWebApi.DTO;
using ProductWebApi.Services;

namespace ProductWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(
            IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products =
                await _service.GetAllAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var product =
                await _service.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(
                    $"Product with Id {id} not found");
            }

            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            ProductCreateDto dto)
        {
            var product =
                await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            ProductUpdateDto dto)
        {
            await _service.UpdateAsync(
                id,
                dto);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
