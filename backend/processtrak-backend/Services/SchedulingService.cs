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
            List<Guid> algorithmIds
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
                ExecuteAlgorithm(processes, algorithm.name);
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

        private void ExecuteAlgorithm(List<Process> processes, string algorithmName)
        {
            // Add scheduling logic for each algorithm
            switch (algorithmName.ToLower())
            {
                case "fcfs":
                    // First-Come, First-Served logic
                    ExecuteFCFS(processes);
                    break;
                case "sjf":
                    // Shortest Job First logic
                    ExecuteSJF(processes);
                    break;
                case "srtf":
                    // Shortest remaining time first
                    break;
                case "priority":
                    // Priority scheduling logic
                    break;
                case "rr":
                    // Round Robin logic
                    break;
            }
        }

        private void ExecuteFCFS(List<Process> processes)
        {
            // Sort processes by Arrival Time
            processes.Sort(
                (p1, p2) =>
                    p1.arrivalTime.GetValueOrDefault().CompareTo(p2.arrivalTime.GetValueOrDefault())
            );

            int currentTime = 0;

            foreach (var process in processes)
            {
                // Wait for the process to arrive
                if (currentTime < process.arrivalTime)
                {
                    currentTime = process.arrivalTime ?? 0;
                }

                // Calculate completion time
                process.completionTime = currentTime + process.burstTime;

                // Calculate turnaround time
                process.turnaroundTime = process.completionTime - process.arrivalTime;

                // Calculate waiting time
                process.waitingTime = process.turnaroundTime - process.burstTime;

                // Update current time
                currentTime = process.completionTime ?? 0;
            }
        }

        private void ExecuteSJF(List<Process> processes)
        {
            int n = processes.Count;
            int completed = 0;
            int currentTime = 0;

            // Sort by arrival time to handle processes in the order they can arrive
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            while (completed < n)
            {
                // Get all processes that have arrived but are not yet completed
                var availableProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.burstTime) // SJF => pick the shortest burst
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    // If no process has arrived yet, simply move time forward
                    currentTime++;
                }
                else
                {
                    // Pick process with the smallest burst time
                    var shortestJob = availableProcesses.First();

                    // Schedule that process
                    shortestJob.completionTime = currentTime + shortestJob.burstTime;
                    shortestJob.turnaroundTime =
                        shortestJob.completionTime - shortestJob.arrivalTime;
                    shortestJob.waitingTime = shortestJob.turnaroundTime - shortestJob.burstTime;
                    shortestJob.isCompleted = true;

                    // Update current time
                    currentTime = shortestJob.completionTime.GetValueOrDefault();
                    completed++;
                }
            }
        }
    }
}
