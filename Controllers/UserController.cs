using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Account;
using ActiveX_Api.Dto.Address;
using ActiveX_Api.Dto.Product;
using ActiveX_Api.Dto.Review;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActiveX_Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context; 
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(AppDbContext context, UserManager<ApiUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            UserDto userDto = user.FromApiUserToUserDto();
            foreach (var role in roles)
            {
                userDto.Role = role; 
            }
            return (user != null ?  Ok(userDto) : NotFound()); 
        }

        [Authorize]
        [HttpGet("reviews")]
        public async Task<IActionResult> GetUserReviews ()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            List<Review> reviews = await _context.Reviews.Include(r => r.Product).Where(r => r.UserId == userId).ToListAsync();
            IEnumerable<ReviewListDto> reviewsDto = reviews.Select(r => r.FromReviewToReviewListDto());
            return Ok(reviewsDto);
        }

        [Authorize]
        [HttpGet("addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            List<Address> addresses = await _context.Address.Where(a => a.UserId == userId).ToListAsync();
            addresses = addresses.Where(a => !a.IsDeleted).ToList();
            IEnumerable<AddressListDto> addressesDto = addresses.Select(a => a.FromAddressToAddressListDto());
            return Ok(addressesDto);
        }

        [Authorize]
        [HttpGet("orders")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            List<Order> orders = await _context.Orders.Include(o => o.Address).Include(o => o.OrderItems).Where(o => o.UserId == userId).ToListAsync();
            return Ok(orders);
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpGet("products")]
        public async Task<IActionResult> GetUserProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var productQuery = _context.Products.Where(p => p.UserId == userId);

            // Get the total count of products for pagination metadata
            var totalProducts = await productQuery.CountAsync();

            // Apply pagination
            var products = await productQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Map to the DTO (assuming FromProductToProductListDto() is an extension method)
            var productsDto = products.Select(p => p.FromProductToProductListDto());

            // Create pagination metadata
            var paginationMetadata = new
            {
                TotalCount = totalProducts,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
            };

            // Return the products along with pagination metadata
            return Ok(new { Products = productsDto, Pagination = paginationMetadata });
        }

    }
}
