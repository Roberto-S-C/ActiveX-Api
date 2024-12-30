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

        //[HttpPost]
        //public async Task<ActionResult<Product>> CreateProduct ([FromBody] CreateProductDto productDto)
        //{
        //   if (productDto == null) {
        //        return BadRequest();
        //   } 


        //}
    }
}
