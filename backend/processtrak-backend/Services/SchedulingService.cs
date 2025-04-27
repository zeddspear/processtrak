using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.Dto;
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
            try
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

                // Set the JSON fields
                scheduleRun.ProcessesJson = JsonSerializer.Serialize(processes);
                scheduleRun.AlgorithmsJson = JsonSerializer.Serialize(algorithms);

                foreach (var algorithm in algorithms)
                {
                    // Execute algorithm logic on processes
                    ExecuteAlgorithm(processes, algorithm.name, timeQuantum);
                }

                // Calculate stats
                scheduleRun.endTime = DateTime.UtcNow;
                scheduleRun.totalExecutionTime = processes.Sum(p => p.completionTime ?? 0);
                scheduleRun.averageWaitingTime = (int)processes.Average(p => p.waitingTime ?? 0);
                scheduleRun.averageTurnaroundTime = (int)
                    processes.Average(p => p.turnaroundTime ?? 0);

                await _context.Schedules.AddAsync(scheduleRun);
                await _context.SaveChangesAsync();

                return scheduleRun;
            }
            catch (JsonException jsonEx)
            {
                // Handle JSON errors specifically
                // Log the error message or rethrow a custom exception with additional context
                throw new Exception(
                    "An error occurred while processing the JSON data: " + jsonEx.Message,
                    jsonEx
                );
            }
            catch (Exception ex)
            {
                // Handle other types of exceptions
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        public async Task<Schedule?> GetScheduleById(Guid id, Guid userId)
        {
            return await _context
                .Schedules.Include(sr => sr.processes)
                .Include(sr => sr.algorithms)
                .FirstOrDefaultAsync(sr => sr.id == id && sr.userId == userId);
        }

        public async Task<List<Schedule>> GetAllSchedulesByUserIdAsync(Guid userId)
        {
            try
            {
                // Fetch all schedules for the specified user, including related processes and algorithms
                var schedules = await _context
                    .Schedules.Include(sr => sr.processes) // Include related processes
                    .Include(sr => sr.algorithms) // Include related algorithms
                    .Where(sr => sr.userId == userId) // Filter by user ID
                    .ToListAsync(); // Execute the query and return the results

                return schedules; // Return the list of schedules
            }
            catch (Exception ex)
            {
                // Handle or log exceptions as needed, re-throw or handle specific cases if necessary
                throw new Exception(
                    "An error occurred while fetching schedules: " + ex.Message,
                    ex
                );
            }
        }

        public async Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid userId)
        {
            try
            {
                // Retrieve the schedule based on the provided ID and user ID
                var scheduleToDelete = await _context
                    .Schedules.Include(sr => sr.processes) // Include related processes if necessary (depends on your deletion logic)
                    .FirstOrDefaultAsync(sr => sr.id == scheduleId && sr.userId == userId);

                // If the schedule does not exist or does not belong to the user, return false
                if (scheduleToDelete == null)
                {
                    return false;
                }

                // Remove the schedule from the context
                _context.Schedules.Remove(scheduleToDelete);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return true indicating successful deletion
                return true;
            }
            catch (Exception ex)
            {
                // Handle or log exceptions as needed
                throw new Exception(
                    "An error occurred while deleting the schedule: " + ex.Message,
                    ex
                );
            }
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
