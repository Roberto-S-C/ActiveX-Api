using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Account;
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

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpGet("products")]
        public async Task<IActionResult> GetUserProducts()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            List<Product> products = await _context.Products.Where(p => p.UserId == userId).ToListAsync();
            IEnumerable<ProductsListDto> productsDto = products.Select(p => p.FromProductToProductListDto());
            return Ok(productsDto);
        }

    }
}
