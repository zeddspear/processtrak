using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class RunScheduleDTO
    {
        [Required(ErrorMessage = "Process IDs are required.")]
        [MinLength(1, ErrorMessage = "At least one Process ID must be provided.")]
        public List<Guid> ProcessIds { get; set; } = new();

        [Required(ErrorMessage = "Algorithm IDs are required.")]
        [MinLength(1, ErrorMessage = "At least one Algorithm ID must be provided.")]
        public List<Guid> AlgorithmIds { get; set; } = new();

        [Range(1, 10, ErrorMessage = "Time Quantum must be between 1 and 10.")]
        public int? TimeQuantum { get; set; } = 3;
    }
}
