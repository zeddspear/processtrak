using System.Text.Json;
using System.Text.Json.Serialization;
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
                var algorithms = await _context
                    .Algorithms.Where(a => algorithmIds.Contains(a.id))
                    .ToListAsync();

                // Always making sure the process being used if they are already completed make isCompleted false again so we could use them properly
                // Reset process states in the database before executing the schedule
                await _context
                    .Processes.Where(p => processIds.Contains(p.id))
                    .ExecuteUpdateAsync(p =>
                        p.SetProperty(x => x.isCompleted, false)
                            .SetProperty(x => x.completionTime, (int?)null)
                            .SetProperty(x => x.remainingTime, (int?)null)
                            .SetProperty(x => x.turnaroundTime, 0)
                            .SetProperty(x => x.waitingTime, 0)
                            .SetProperty(x => x.responseTime, (int?)null)
                            .SetProperty(x => x.startTime, (int?)null)
                            .SetProperty(x => x.State, enums.ProcessState.Ready)
                    );

                // Refresh process data from database to ensure we have the updated state
                var processes = await _context
                    .Processes.Where(p => processIds.Contains(p.id) && p.userId == userId)
                    .OrderBy(p => p.arrivalTime)
                    .ToListAsync();

                var scheduleRun = new Schedule
                {
                    userId = userId,
                    startTime = DateTime.UtcNow,
                    processes = processes,
                    algorithms = algorithms,
                };

                var executionLog = new List<ExecutionLogEntry>();

                foreach (var algorithm in algorithms)
                {
                    // Execute algorithm logic on processes
                    ExecuteAlgorithm(processes, algorithm.name, timeQuantum, executionLog);
                }

                // Set the JSON fields
                scheduleRun.ProcessesJson = JsonSerializer.Serialize(processes);
                scheduleRun.AlgorithmsJson = JsonSerializer.Serialize(algorithms);
                scheduleRun.ExecutionLogJson = JsonSerializer.Serialize(executionLog);

                // Calculate stats
                scheduleRun.endTime = DateTime.UtcNow;
                scheduleRun.totalExecutionTime = processes.Sum(p => p.completionTime ?? 0);
                scheduleRun.averageWaitingTime = (int)processes.Average(p => p.waitingTime ?? 0);
                scheduleRun.averageTurnaroundTime = (int)
                    processes.Average(p => p.turnaroundTime ?? 0);

                await _context.Schedules.AddAsync(scheduleRun);
                await _context.SaveChangesAsync();

                var user = await _context
                    .Users.Where(u => u.id == userId) // Your condition to find the user
                    .Select(u => new
                    {
                        u.id,
                        u.name,
                        u.email,
                        u.phone,
                        u.isGuest,
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                //if user is guest delete its processes
                if (user.isGuest)
                {
                    await _context
                        .Processes.Where(p => processIds.Contains(p.id))
                        .ExecuteDeleteAsync();
                }

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

        public async Task<Schedule> ReRunScheduleAsync(
            Guid scheduleId,
            Guid userId,
            List<Guid> processIds,
            List<Guid> algorithmIds,
            int timeQuantum
        )
        {
            try
            {
                var schedule = await _context
                    .Schedules.Include(s => s.processes)
                    .Include(s => s.algorithms)
                    .FirstOrDefaultAsync(s => s.id == scheduleId && s.userId == userId);

                if (schedule == null)
                {
                    throw new Exception("Schedule not found for the given user.");
                }

                // Get updated algorithms
                var algorithms = await _context
                    .Algorithms.Where(a => algorithmIds.Contains(a.id))
                    .ToListAsync();

                // Reset selected processes
                await _context
                    .Processes.Where(p => processIds.Contains(p.id))
                    .ExecuteUpdateAsync(p =>
                        p.SetProperty(x => x.isCompleted, false)
                            .SetProperty(x => x.completionTime, (int?)null)
                            .SetProperty(x => x.remainingTime, (int?)null)
                            .SetProperty(x => x.turnaroundTime, 0)
                            .SetProperty(x => x.waitingTime, 0)
                            .SetProperty(x => x.responseTime, (int?)null)
                            .SetProperty(x => x.startTime, (int?)null)
                            .SetProperty(x => x.State, enums.ProcessState.Ready)
                    );

                var processes = await _context
                    .Processes.Where(p => processIds.Contains(p.id) && p.userId == userId)
                    .OrderBy(p => p.arrivalTime)
                    .ToListAsync();

                var executionLog = new List<ExecutionLogEntry>();

                foreach (var algorithm in algorithms)
                {
                    ExecuteAlgorithm(processes, algorithm.name, timeQuantum, executionLog);
                }

                // Update schedule data
                schedule.startTime = DateTime.UtcNow;
                schedule.endTime = DateTime.UtcNow;
                schedule.processes = processes;
                schedule.algorithms = algorithms;

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    WriteIndented = true,
                };

                schedule.ProcessesJson = JsonSerializer.Serialize(processes, options);
                schedule.AlgorithmsJson = JsonSerializer.Serialize(algorithms, options);
                schedule.ExecutionLogJson = JsonSerializer.Serialize(executionLog, options);
                schedule.totalExecutionTime = processes.Sum(p => p.completionTime ?? 0);
                schedule.averageWaitingTime = (int)processes.Average(p => p.waitingTime ?? 0);
                schedule.averageTurnaroundTime = (int)processes.Average(p => p.turnaroundTime ?? 0);

                await _context.SaveChangesAsync();

                return schedule;
            }
            catch (Exception ex)
            {
                throw new Exception("Error re-running schedule: " + ex.Message, ex);
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
                // Fetch all schedules for the specified user
                var schedules = await _context
                    .Schedules.Where(sr => sr.userId == userId) // Filter by user ID
                    .Select(sr => new Schedule
                    {
                        id = sr.id,
                        userId = sr.userId,
                        startTime = sr.startTime,
                        endTime = sr.endTime,
                        totalExecutionTime = sr.totalExecutionTime,
                        averageWaitingTime = sr.averageWaitingTime,
                        averageTurnaroundTime = sr.averageTurnaroundTime,
                        ProcessesJson = sr.ProcessesJson,
                        AlgorithmsJson = sr.AlgorithmsJson,
                        processes = sr.processes ?? new List<Process>(), // Ensure a non-null list
                        algorithms = sr.algorithms ?? new List<Algorithm>(), // Ensure a non-null list
                    })
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
            int timeQuantum,
            List<ExecutionLogEntry> executionLog
        )
        {
            // Add scheduling logic for each algorithm
            switch (algorithmName.ToLower())
            {
                case "fcfs":
                    // First-Come, First-Served logic
                    StaticAlgorithmService.ExecuteFCFS(processes, executionLog);
                    break;
                case "sjf":
                    // Shortest Job First logic
                    StaticAlgorithmService.ExecuteSJF(processes, executionLog);
                    break;
                case "srtf":
                    // Shortest remaining time first
                    StaticAlgorithmService.ExecuteSRTF(processes, executionLog);
                    break;
                case "priority_non_preemptive":
                    // Priority (non-pre-emptive) scheduling logic
                    StaticAlgorithmService.RunPrioritySchedulingNonPreemptive(
                        processes,
                        executionLog
                    );
                    break;
                case "priority_preemptive":
                    // Priority (non-pre-emptive) scheduling logic
                    StaticAlgorithmService.RunPrioritySchedulingPreemptive(processes, executionLog);
                    break;
                case "rr":
                    // Round Robin logic
                    StaticAlgorithmService.RunRoundRobin(processes, timeQuantum, executionLog);
                    break;
                default:
                    throw new ArgumentException(
                        $"Algorithm '{algorithmName}' not found in the system."
                    );
            }
        }
    }
}
