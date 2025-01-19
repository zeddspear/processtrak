using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class CreateProcessDTO
    {
        [Required(ErrorMessage = "Process ID is required.")]
        [StringLength(50, ErrorMessage = "Process ID cannot exceed 50 characters.")]
        public string ProcessId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrival Time is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Arrival Time must be a non-negative value.")]
        public int ArrivalTime { get; set; }

        [Required(ErrorMessage = "Burst Time is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Burst Time must be at least 1.")]
        public int BurstTime { get; set; }

        [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10.")]
        public int? Priority { get; set; }
    }
}
