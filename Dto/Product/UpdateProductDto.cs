using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Product
{
    public class UpdateProductDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name can't be more than 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(300, ErrorMessage = "Description can't be more than 300 characters.")]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public IFormFile? File3DModel { get; set; }
        
    }
}
