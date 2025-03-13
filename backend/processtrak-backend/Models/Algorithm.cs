namespace processtrak_backend.Models
{
    public class Algorithm : BaseEntity
    {
        public required string name { get; set; }
        public string? description { get; set; }
        public string? displayName { get; set; } = null; // New property added
        public bool? requiresTimeQuantum { get; set; } = null; // New boolean property added
        public List<Schedule>? scheduleRuns { get; set; } = new();
    }
}
