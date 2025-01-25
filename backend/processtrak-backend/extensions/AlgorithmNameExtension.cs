using processtrak_backend.Dto;

namespace processtrak_backend.Extensions
{
    public static class AlgorithmNameExtension
    {
        public static string ToFriendlyString(this AlgorithmName algorithmName)
        {
            return algorithmName switch
            {
                AlgorithmName.FCFS => "fcfs",
                AlgorithmName.SJF => "sjf",
                AlgorithmName.SRTF => "srtf",
                AlgorithmName.RoundRobin => "round_robin",
                AlgorithmName.PriorityNonPreemptive => "priority_non_preemptive",
                AlgorithmName.PriorityPreemptive => "priority_preemptive",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(algorithmName),
                    algorithmName,
                    null
                ),
            };
        }
    }
}
