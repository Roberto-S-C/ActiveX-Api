using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Account
{
    public class RegisterUserDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
