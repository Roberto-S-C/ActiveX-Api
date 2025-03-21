using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveX_Api.Dto.Order
{
    public class CreateOrderDto
    {
        [Required]
        public string? StripeSessionId { get; set; }
        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; set; }
        [Required]
        public string Status { get; set; } = "pending"; // "pending", "paid", "failed"
        [Required]
        public int AddressId { get; set; }
    }
}
