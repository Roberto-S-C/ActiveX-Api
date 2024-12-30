using ActiveX_Api.Dto.Category;
using ActiveX_Api.Dto.Review;
using ActiveX_Api.Models;

namespace ActiveX_Api.Dto.Product
{
    public class GetProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? File3DModel { get; set; }
        public CategoriesListDto Category { get; set; }
        public ICollection<ReviewListDto> Reviews { get; set; }
    }
}
