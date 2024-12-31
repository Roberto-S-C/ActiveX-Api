using ActiveX_Api.Dto.Review;
using ActiveX_Api.Models;

namespace ActiveX_Api.Mappers
{
    public static class ReviewMapper
    {
        public static ReviewListDto FromReviewToReviewListDto (this Review review)
        {
            return new ReviewListDto
            {
                Id = review.Id,
                Title = review.Title,
                Content = review.Content,
                Stars = review.Stars,
                CreatedAt = review.CreatedAt,
                ProductId = review.ProductId
            };
        }
    }
}
