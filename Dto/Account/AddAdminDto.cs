using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Account
{
    public class AddAdminDto
    {
        [Required]
        [MaxLength(255)]
        public string? UserName{ get; set; }
    }
}
