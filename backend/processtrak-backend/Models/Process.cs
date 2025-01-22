using processtrak_backend.enums;

namespace processtrak_backend.Models
{
    public class Process : BaseEntity
    {
        public Guid userId { get; set; } // Foreign Key to Users table
        public string? processId { get; set; } = string.Empty; // User-defined process identifier
        public required string name { get; set; }
        public int? arrivalTime { get; set; }
        public int? burstTime { get; set; }
        public int? priority { get; set; } // Optional field for Priority Scheduling
        public int? remainingTime { get; set; } // For preemptive algorithms
        public int? startTime { get; set; } // Optional, for calculating response time
        public int? completionTime { get; set; }
        public int? responseTime { get; set; } // Time when the process first starts execution
        public int? turnaroundTime { get; set; }
        public int? waitingTime { get; set; }
        public bool? isCompleted { get; set; } = false;

        public ProcessState State { get; set; } = ProcessState.Ready;

        // Navigation property
        public User? user { get; set; }
    }
}
