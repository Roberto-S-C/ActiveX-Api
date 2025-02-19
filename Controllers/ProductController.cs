using System.Runtime.CompilerServices;
using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Product;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using ActiveX_Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActiveX_Api.Controllers
{
    [Route("/api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context; 
        private readonly UserManager<ApiUser> _userManager; 
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _enviroment;

        private readonly long _fileSizeLimit = 50 * 1024 * 1024;

        public ProductController(AppDbContext context, UserManager<ApiUser> userManager, IConfiguration config, IWebHostEnvironment enviroment)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _enviroment = enviroment;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? name, [FromQuery] int? category)
        {
            // Start with the query for all products
            var productQuery = _context.Products.Include(p => p.Category).AsQueryable();

            // If a category is provided, filter by category
            if (category.HasValue && category.Value != 0)
            {
                var categoryModel = await _context.Categories.FindAsync(category.Value);
                if (categoryModel == null) return NotFound($"Category {category} not found");

                productQuery = productQuery.Where(p => p.CategoryId == category.Value);
            }

            // If a product name is provided, filter by product name
            if (!string.IsNullOrEmpty(name))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(name));
            }

            // Execute the query and retrieve the filtered list of products
            var productList = await productQuery.ToListAsync();

            // Map to the DTO (assuming FromProductToProductListDto() is an extension method)
            var productDtos = productList.Select(p => p.FromProductToProductListDto());

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
           var product = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);
           return (product == null ? NotFound() : Ok(product.FromProductToGetProductDto()));
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct ([FromForm] CreateProductDto productDto)
        {
            var category =  await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");

            if(FileValidation.ValidateFile(productDto.File3DModel)) {
                string fileName = FileValidation.GenerateFileName();
                string filePath = Path.Combine(_config["StoredFiles"], fileName);
                string absolutePath = Path.Combine(_enviroment.WebRootPath, filePath); ;

                using (var stream = System.IO.File.Create(absolutePath))
                {
                    await productDto.File3DModel.CopyToAsync(stream);
                }

                var product = productDto.FromCreateProductDtoToProduct();
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
                product.UserId = userId;
                product.File3DModel = filePath;

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            else
            {
                return BadRequest("Can't send empty files");
            }
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromForm] UpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            if(product.UserId != userId) return Unauthorized("Only owner can edit product"); 

            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");

            if (FileValidation.ValidateFile(productDto.File3DModel)) {
                string fileName = FileValidation.GenerateFileName();
                string filePath = Path.Combine(_config["StoredFiles"], fileName);
                string absolutePath = Path.Combine(_enviroment.WebRootPath, filePath); ;

                using (var stream = System.IO.File.Create(absolutePath))
                {
                    await productDto.File3DModel.CopyToAsync(stream);
                }
                
                product.File3DModel = filePath;
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;
            product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null) return NotFound();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            if(product.UserId != userId) return Unauthorized("Only owner can delete product"); 

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
