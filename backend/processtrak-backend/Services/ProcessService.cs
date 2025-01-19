using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.Dto;
using processtrak_backend.interfaces;
using processtrak_backend.Models;

namespace processtrak_backend.Services
{
    public class ProcessService : IProcessService
    {
        private readonly AppDbContext _context;

        public ProcessService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Process>> GetAllProcessesAsync(Guid userId)
        {
            return await _context
                .Processes.Where(p => p.userId == userId)
                .OrderBy(p => p.arrivalTime)
                .ToListAsync();
        }

        public async Task<Process?> GetProcessByIdAsync(Guid userId, Guid processId)
        {
            return await _context.Processes.FirstOrDefaultAsync(p =>
                p.id == processId && p.userId == userId
            );
        }

        public async Task<Process> CreateProcessAsync(Guid userId, CreateProcessDTO dto)
        {
            var process = new Process
            {
                userId = userId,
                processId = dto.ProcessId,
                name = dto.name,
                arrivalTime = dto.ArrivalTime,
                burstTime = dto.BurstTime,
                priority = dto.Priority,
            };

            await _context.Processes.AddAsync(process);
            await _context.SaveChangesAsync();

            return process;
        }

        public async Task<bool> UpdateProcessAsync(
            Guid userId,
            Guid processId,
            UpdateProcessDTO dto
        )
        {
            var process = await _context.Processes.FirstOrDefaultAsync(p =>
                p.id == processId && p.userId == userId
            );

            if (process == null)
                return false;

            if (dto.ArrivalTime.HasValue)
                process.arrivalTime = dto.ArrivalTime.Value;

            if (dto.BurstTime.HasValue)
                process.burstTime = dto.BurstTime.Value;

            if (dto.Priority.HasValue)
                process.priority = dto.Priority;

            process.updatedAt = DateTime.UtcNow;

            _context.Processes.Update(process);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProcessAsync(Guid userId, Guid processId)
        {
            var process = await _context.Processes.FirstOrDefaultAsync(p =>
                p.id == processId && p.userId == userId
            );

            if (process == null)
                return false;

            _context.Processes.Remove(process);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
