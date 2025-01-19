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
                AlgorithmName.RoundRobin => "round_robin",
                AlgorithmName.Priority => "priority",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(algorithmName),
                    algorithmName,
                    null
                ),
            };
        }
    }
}
