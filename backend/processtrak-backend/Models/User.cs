namespace processtrak_backend.Models
{
    public class User : BaseEntity
    {
        public required string name { get; set; }

        public required string email { get; set; }

        public required string password { get; set; }

        public required string phone { get; set; }

        public bool isGuest { get; set; } = false;

        public ICollection<UserSession>? UserSessions { get; set; }
        public ICollection<Process>? Processes { get; set; }
    }
}
