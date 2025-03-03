﻿using ActiveX_Api.Constants;
using ActiveX_Api.Dto.Review;
using ActiveX_Api.Mappers;
using ActiveX_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var review = await _context.Reviews.Include(r => r.Product).FirstOrDefaultAsync(r => r.Id == id);

            return (review == null) ? NotFound() : Ok(review.FromReviewToReviewListDto()); 
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateReview ([FromBody] CreateReviewDto reviewDto)
        {
            var product = await _context.Products.FindAsync(reviewDto.ProductId);
            if (product == null) return BadRequest("Invalid ProductId");

            var review = reviewDto.FromCreateReviewDtoToReview();
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.UserName)?.Value;
            review.UserId = userId;
            review.UserName = userName;

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review.FromReviewToReviewListDto());
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateReview([FromBody] UpdateReviewDto reviewDto, [FromRoute] int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            if (review.UserId != userId) return Unauthorized("Only owner can edit review");

            review.Title = reviewDto.Title;
            review.Content = reviewDto.Content;
            review.Stars = reviewDto.Stars;
            review.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview([FromRoute] int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimNames.Id)?.Value;
            if (review.UserId != userId) return Unauthorized("Only owner can delete review");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
