using System.ComponentModel.DataAnnotations.Schema;

namespace ActiveX_Api.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
