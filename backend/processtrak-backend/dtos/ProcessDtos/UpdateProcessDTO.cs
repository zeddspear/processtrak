using System.ComponentModel.DataAnnotations;

namespace processtrak_backend.Dto
{
    public class UpdateProcessDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "Arrival Time must be a non-negative value.")]
        public int? ArrivalTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Burst Time must be at least 1.")]
        public int? BurstTime { get; set; }

        [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10.")]
        public int? Priority { get; set; }
    }
}
