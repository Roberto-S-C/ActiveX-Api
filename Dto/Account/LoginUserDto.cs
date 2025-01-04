using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Account
{
    public class LoginUserDto
    {
        [Required]
        [MaxLength(255)]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
