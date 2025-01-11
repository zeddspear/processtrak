using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.DTO
{
    public class UserRegistrationDto
    {
        [Required]
        public required string name { get; set; }

        [Required]
        [EmailAddress]
        public required string email { get; set; }

        [Required]
        [MinLength(8)]
        public required string password { get; set; }

        [Required]
        [Phone]
        public required string phone { get; set; }
    }
}
