using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public required string email { get; set; }

        [Required]
        public required string password { get; set; }
    }
}
