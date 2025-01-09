using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Product
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name can't be more than 100 characters.")]
        public string? Name { get; set; }
        [Required]
        [MaxLength(300, ErrorMessage = "Description can't be more than 300 characters.")]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public IFormFile? File3DModel { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
