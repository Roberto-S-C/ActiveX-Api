using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Review
{
    public class CreateReviewDto
    {
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [Required]
        [MaxLength(300)]
        public string? Content { get; set; }
        [Required]
        [Range(1, 5)]
        public int Stars {  get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
