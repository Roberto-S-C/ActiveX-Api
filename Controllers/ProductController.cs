using System.Runtime.CompilerServices;
using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Product;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using ActiveX_Api.Services;
using ActiveX_Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace ActiveX_Api.Controllers
{
    [Route("/api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context; 
        private readonly UserManager<ApiUser> _userManager; 
        private readonly IConfiguration _config;
        private readonly BlobStorageService _blobStorageService;

        public ProductController(AppDbContext context, UserManager<ApiUser> userManager, IConfiguration config, BlobStorageService blobStorageService)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? name, [FromQuery] int? category, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
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

            // Get the total count of products for pagination metadata
            var totalProducts = await productQuery.CountAsync();

            // Apply pagination
            var products = await productQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Map to the DTO (assuming FromProductToProductListDto() is an extension method)
            var productDtos = products.Select(p => p.FromProductToProductListDto());

            // Create pagination metadata
            var paginationMetadata = new
            {
                TotalCount = totalProducts,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
            };

            // Return the products along with pagination metadata
            return Ok(new { Products = productDtos, Pagination = paginationMetadata });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
           var product = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);
           return (product == null ? NotFound() : Ok(product.FromProductToGetProductDto()));
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPost]
        public async Task<ActionResult<Models.Product>> CreateProduct ([FromForm] CreateProductDto productDto)
        {
            var category =  await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null) return BadRequest("Invalid CategoryId");

            //Create Product in Stripe
            var productService = new ProductService();
            var priceService = new PriceService();

            var stripeProduct = await productService.CreateAsync(new ProductCreateOptions
            {
                Name = productDto.Name,
            });

            var stripePrice = await priceService.CreateAsync(new PriceCreateOptions
            {
                Product = stripeProduct.Id,
                UnitAmount = (long)(productDto.Price * 100), // Convert to cents
                Currency = "usd"
            });

            //Saving 3D Model
            if (FileValidation.ValidateFile(productDto.File3DModel)) {
                string fileName = FileValidation.GenerateFileName();
                string blobUrl = await _blobStorageService.UploadFileAsync(productDto.File3DModel, fileName);   
                
                var product = productDto.FromCreateProductDtoToProduct();
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
                product.UserId = userId;
                product.File3DModel = blobUrl;
                product.StripeProductId = stripeProduct.Id;
                product.StripePriceId = stripePrice.Id;

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

            // Update product in Stripe
            var productService = new ProductService();
            var priceService = new PriceService();

            var stripeProduct = await productService.UpdateAsync(product.StripeProductId, new ProductUpdateOptions
            {
                Name = productDto.Name,
            });

            // If the price has changed, create a new price in Stripe
            string updatedStripePriceId = product.StripePriceId;
            if (product.Price != productDto.Price)
            {
                var stripePrice = await priceService.CreateAsync(new PriceCreateOptions
                {
                    Product = stripeProduct.Id,
                    UnitAmount = (long)(productDto.Price * 100), // Convert to cents
                    Currency = "usd"
                });

                updatedStripePriceId = stripePrice.Id;
            }

            if (FileValidation.ValidateFile(productDto.File3DModel)) {
                var blobUrl = await _blobStorageService.UploadFileAsync(productDto.File3DModel, product.File3DModel);

                product.File3DModel = blobUrl; 
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;
            product.StripePriceId = updatedStripePriceId;
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

            var productService = new ProductService();
            try
            {
                await productService.UpdateAsync(product.StripeProductId, new ProductUpdateOptions
                {
                    Active = false
                });

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (StripeException e) {
                return BadRequest(new { error = e.Message }); 
            }

            return NoContent(); 
        }
    }
}
