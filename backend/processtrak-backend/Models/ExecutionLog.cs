namespace processtrak_backend.Models;

public class ExecutionLogEntry
{
    public Guid processId { get; set; }
    public int startTime { get; set; }
    public int endTime { get; set; }
}
