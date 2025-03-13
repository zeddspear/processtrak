using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class AddAlgorithmDTO
    {
        [Required(ErrorMessage = "Algorithm name is required.")]
        [EnumDataType(
            typeof(AlgorithmName),
            ErrorMessage = "Invalid algorithm name. Valid values are: fcfs, sjf, srtf, round_robin, priority_non_preemptive, priority_preemptive."
        )]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public string? DisplayName { get; set; }

        public bool? RequiresTimeQuantum { get; set; } = null;
    }

    public enum AlgorithmName
    {
        FCFS, // First-Come, First-Served
        SJF, // Shortest Job First
        SRTF,
        RoundRobin,
        PriorityNonPreemptive,
        PriorityPreemptive,
    }
}
