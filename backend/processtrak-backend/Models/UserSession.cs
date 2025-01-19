namespace processtrak_backend.Models
{
    public class UserSession : BaseEntity
    {
        public required Guid userId { get; set; }
        public required string token { get; set; }
        public required DateTime expiryTime { get; set; }

        public required User user { get; set; }
    }
}
