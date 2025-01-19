namespace processtrak_backend.Models
{
    public class Algorithm : BaseEntity
    {
        public required string name { get; set; }
        public string? description { get; set; }
        public List<Schedule>? scheduleRuns { get; set; } = new();
    }
}
