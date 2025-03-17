using System.ComponentModel.DataAnnotations;

namespace ActiveX_Api.Dto.Address
{
    public class CreateAddressDto
    {
        [Required]
        [MaxLength(60, ErrorMessage = "Name can't be more than 60 characters.")]
        public string? FullName { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Street can't be more than 100 characters.")]
        public string? Street { get; set; }
        [Required]
        [MaxLength(7, ErrorMessage = "Number can't be more than 7 characters.")]
        public string? Number { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "City can't be more than 50 characters.")]
        public string? City { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "State can't be more than 50 characters.")]
        public string? State { get; set; }
        [Required]
        [MaxLength(9, ErrorMessage = "Zip code can't be more than 9 characters.")]
        public string? PostalCode { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Country can't be more than 30 characters.")]
        public string? Country { get; set; }
        [Required]
        [MaxLength(15, ErrorMessage = "Phone can't be more than 15 characters.")]
        public string? Phone { get; set; }
    }
}
