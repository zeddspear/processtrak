namespace processtrak_backend.Models
{
    public class Process : BaseEntity
    {
        public Guid userId { get; set; } // Foreign Key to Users table
        public string processId { get; set; } = string.Empty; // User-defined process identifier
        public int arrivalTime { get; set; }
        public int burstTime { get; set; }
        public int? priority { get; set; } // Optional field for Priority Scheduling

        // Navigation property
        public User? user { get; set; }
    }
}
