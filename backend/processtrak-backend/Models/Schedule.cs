namespace processtrak_backend.Models
{
    public class Schedule : BaseEntity
    {
        public Guid userId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public List<Process> processes { get; set; } = new();
        public List<Algorithm> algorithms { get; set; } = new();

        public int totalExecutionTime { get; set; }
        public int averageWaitingTime { get; set; }
        public int averageTurnaroundTime { get; set; }
    }
}
