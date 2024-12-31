using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Category
{
    public class UpdateCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
