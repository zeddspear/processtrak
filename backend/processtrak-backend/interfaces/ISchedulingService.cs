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

        Task<Schedule> ReRunScheduleAsync(
            Guid scheduleId,
            Guid userId,
            List<Guid> processIds,
            List<Guid> algorithmIds,
            int timeQuantum
        );

        Task<Schedule?> GetScheduleById(Guid id, Guid userId);

        Task<List<Schedule>> GetAllSchedulesByUserIdAsync(Guid userId);

        Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid userId);
    }
}
