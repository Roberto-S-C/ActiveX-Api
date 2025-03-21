using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Checkout
{
    public class Checkout
    {
        [Required]
        public string? StripePriceId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
