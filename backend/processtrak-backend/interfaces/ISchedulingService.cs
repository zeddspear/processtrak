using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using processtrak_backend.Models;

namespace processtrak_backend.interfaces
{
    public interface ISchedulingService
    {
        Task<Schedule> RunScheduleAsync(
            Guid userId,
            List<Guid> processIds,
            List<Guid> algorithmIds
        );

        Task<Schedule?> GetScheduleById(Guid id, Guid userId);
    }
}
