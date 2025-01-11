namespace processtrak_backend.Models
{
    public class OtpCode : BaseEntity
    {
        public required string email { get; set; }

        public required string code { get; set; }
        public DateTime expiryTime { get; set; }
    }
}
