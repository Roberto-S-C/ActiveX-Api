using Microsoft.AspNetCore.Identity;

namespace ActiveX_Api.Models
{
    public class ApiUser : IdentityUser
    {
        public ICollection<Product> Products { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
