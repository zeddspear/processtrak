using System.Text.Json;

namespace processtrak_backend.Models
{
    public class Schedule : BaseEntity
    {
        public Guid userId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public List<Process> processes { get; set; } = new();
        public List<Algorithm> algorithms { get; set; } = new();

        // New columns for JSON representation    // Store JSON strings directly for processes and algorithms
        public string ProcessesJson { get; set; } = string.Empty; // JSON representation of processes
        public string AlgorithmsJson { get; set; } = string.Empty; // JSON representation of algorithms

        public int totalExecutionTime { get; set; }
        public int averageWaitingTime { get; set; }
        public int averageTurnaroundTime { get; set; }
    }
}
