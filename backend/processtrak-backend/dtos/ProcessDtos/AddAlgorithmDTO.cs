using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class AddAlgorithmDTO
    {
        [Required(ErrorMessage = "Algorithm name is required.")]
        [EnumDataType(
            typeof(AlgorithmName),
            ErrorMessage = "Invalid algorithm name. Valid values are: fcfs, sjf, round_robin, priority."
        )]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }

    public enum AlgorithmName
    {
        FCFS, // First-Come, First-Served
        SJF, // Shortest Job First
        RoundRobin,
        Priority,
    }
}
