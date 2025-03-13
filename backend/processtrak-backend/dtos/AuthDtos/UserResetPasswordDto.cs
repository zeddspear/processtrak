using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace processtrak_backend.Dto
{
    public class UserResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string email { get; set; }

        [Required]
        [NotNull]
        public required string code { get; set; }

        [Required]
        [NotNull]
        public required string newPassword { get; set; }
    }
}
