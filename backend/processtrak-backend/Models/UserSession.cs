namespace processtrak_backend.Models
{
    public class UserSession : BaseEntity
    {
        public required Guid UserId { get; set; }
        public required string Token { get; set; }
        public required DateTime ExpiryTime { get; set; }

        public required User User { get; set; }
    }
}
