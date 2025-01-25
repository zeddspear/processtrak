using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.interfaces;
using processtrak_backend.Models;

namespace processtrak_backend.Services
{
    public class SchedulingService : ISchedulingService
    {
        private readonly AppDbContext _context;

        public SchedulingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> RunScheduleAsync(
            Guid userId,
            List<Guid> processIds,
            List<Guid> algorithmIds,
            int timeQuantum
        )
        {
            var processes = await _context
                .Processes.Where(p => processIds.Contains(p.id) && p.userId == userId)
                .OrderBy(p => p.arrivalTime)
                .ToListAsync();

            var algorithms = await _context
                .Algorithms.Where(a => algorithmIds.Contains(a.id))
                .ToListAsync();

            var scheduleRun = new Schedule
            {
                userId = userId,
                startTime = DateTime.UtcNow,
                processes = processes,
                algorithms = algorithms,
            };

            foreach (var algorithm in algorithms)
            {
                // Execute algorithm logic on processes
                ExecuteAlgorithm(processes, algorithm.name, timeQuantum);
            }

            // Calculate stats
            scheduleRun.endTime = DateTime.UtcNow;
            scheduleRun.totalExecutionTime = processes.Sum(p => p.completionTime ?? 0);
            scheduleRun.averageWaitingTime = (int)processes.Average(p => p.waitingTime ?? 0);
            scheduleRun.averageTurnaroundTime = (int)processes.Average(p => p.turnaroundTime ?? 0);

            await _context.Schedules.AddAsync(scheduleRun);
            await _context.SaveChangesAsync();

            return scheduleRun;
        }

        public async Task<Schedule?> GetScheduleById(Guid id, Guid userId)
        {
            return await _context
                .Schedules.Include(sr => sr.processes)
                .Include(sr => sr.algorithms)
                .FirstOrDefaultAsync(sr => sr.id == id && sr.userId == userId);
        }

        private void ExecuteAlgorithm(
            List<Process> processes,
            string algorithmName,
            int timeQuantum
        )
        {
            // Add scheduling logic for each algorithm
            switch (algorithmName.ToLower())
            {
                case "fcfs":
                    // First-Come, First-Served logic
                    StaticAlgorithmService.ExecuteFCFS(processes);
                    break;
                case "sjf":
                    // Shortest Job First logic
                    StaticAlgorithmService.ExecuteSJF(processes);
                    break;
                case "srtf":
                    // Shortest remaining time first
                    StaticAlgorithmService.ExecuteSRTF(processes);
                    break;
                case "priority_non_preemptive":
                    // Priority (non-pre-emptive) scheduling logic
                    StaticAlgorithmService.RunPrioritySchedulingNonPreemptive(processes);
                    break;
                case "priority_preemptive":
                    // Priority (non-pre-emptive) scheduling logic
                    StaticAlgorithmService.RunPrioritySchedulingPreemptive(processes);
                    break;
                case "rr":
                    // Round Robin logic
                    StaticAlgorithmService.RunRoundRobin(processes, timeQuantum);
                    break;
            }
        }
    }
}
