using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
