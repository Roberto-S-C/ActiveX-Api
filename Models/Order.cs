using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveX_Api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? StripeSessionId { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; set; }
        public string Status { get; set; } = "pending"; // "pending", "paid", "failed"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public string? UserId { get; set; }
        public ApiUser User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
