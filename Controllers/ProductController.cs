using ActiveX_Api.Dto.Product;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActiveX_Api.Controllers
{
    [Route("/api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
       private readonly AppDbContext _context; 

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductsListDto>> GetProducts()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return products.Select(p => p.FromProductToProductListDto());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
           var product = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);
           return (product == null ? NotFound() : Ok(product.FromProductToGetProductDto()));
        } 

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct ([FromBody] CreateProductDto productDto)
        {
            var category =  await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");

            var product = productDto.FromCreateProductDtoToProduct();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.File3DModel = productDto.File3DModel;
            product.CategoryId = productDto.CategoryId;
            product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
