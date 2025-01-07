using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveX_Api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Price { get; set; }
        public string? File3DModel { get; set; }
        public DateOnly CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string? UserId { get; set; }
        public ApiUser User { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
