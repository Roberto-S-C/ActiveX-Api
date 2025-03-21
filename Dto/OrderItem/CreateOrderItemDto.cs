using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveX_Api.Dto.OrderItem
{
    public class CreateOrderItemDto
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than or equal to 1.")]
        public int Quantity { get; set; }
    }
}
