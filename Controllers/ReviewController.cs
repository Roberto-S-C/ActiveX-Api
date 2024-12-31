using ActiveX_Api.Dto.Review;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ActiveX_Api.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetReview (int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            return (review == null) ? NotFound() : Ok(review.FromReviewToReviewListDto()); 
        }

        [HttpPost]
        public async Task<ActionResult> CreateReview ([FromBody] CreateReviewDto reviewDto)
        {
            var product = await _context.Products.FindAsync(reviewDto.ProductId);
            if (product == null) return BadRequest("Invalid ProductId");

            var review = reviewDto.FromCreateReviewDtoToReview(); 
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review.FromReviewToReviewListDto());
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateReview([FromBody] UpdateReviewDto reviewDto, [FromRoute] int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            review.Title = reviewDto.Title;
            review.Content = reviewDto.Content;
            review.Stars = reviewDto.Stars;
            review.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview([FromRoute] int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
