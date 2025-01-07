using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Category;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActiveX_Api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory([FromRoute] int id)
        {
            var category = await _context.Categories.FindAsync(id); 
            return (category == null) ? NotFound() : Ok(category); 
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            var category = categoryDto.FromCreateCategoryDtoToCategory();
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            category.UserId = userId;

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategoryDto categoryDto, int id)
        {
            var category = await _context.Categories.FindAsync(id);            
            if (category == null) return NotFound();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            if(category.UserId != userId) return Unauthorized("Only owner can edit category");

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory([FromRoute] int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            if(category.UserId != userId) return Unauthorized("Only owner can delete category");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        } 
    }
}
