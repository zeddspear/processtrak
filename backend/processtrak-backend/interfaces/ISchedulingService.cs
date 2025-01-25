using processtrak_backend.Models;

namespace processtrak_backend.interfaces
{
    public interface ISchedulingService
    {
        Task<Schedule> RunScheduleAsync(
            Guid userId,
            List<Guid> processIds,
            List<Guid> algorithmIds,
            int timeQuantum
        );

        Task<Schedule?> GetScheduleById(Guid id, Guid userId);
    }
}
