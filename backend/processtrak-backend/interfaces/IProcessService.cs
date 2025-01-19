using processtrak_backend.Dto;
using processtrak_backend.Models;

namespace processtrak_backend.interfaces
{
    public interface IProcessService
    {
        Task<IEnumerable<Process>> GetAllProcessesAsync(Guid userId);
        Task<Process?> GetProcessByIdAsync(Guid userId, Guid processId);
        Task<Process> CreateProcessAsync(Guid userId, CreateProcessDTO dto);
        Task<bool> UpdateProcessAsync(Guid userId, Guid processId, UpdateProcessDTO dto);
        Task<bool> DeleteProcessAsync(Guid userId, Guid processId);
    }
}
