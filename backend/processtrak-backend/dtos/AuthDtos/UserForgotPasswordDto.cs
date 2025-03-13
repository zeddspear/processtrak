using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class UserForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string email { get; set; }
    }
}
