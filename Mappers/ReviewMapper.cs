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
                ProductId = review.ProductId,
                ProductName = review.Product.Name,
                UserName = review.UserName
            };
        }

        public static Review FromCreateReviewDtoToReview (this CreateReviewDto review)
        {
            return new Review
            {
                Title = review.Title,
                Content = review.Content,
                Stars = review.Stars,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                ProductId = review.ProductId
            };
        }
    }
}
