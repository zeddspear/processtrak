using processtrak_backend.Api.data;
using processtrak_backend.Dto;
using processtrak_backend.Extensions;
using processtrak_backend.interfaces;
using processtrak_backend.Models;

namespace processtrak_backend.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly AppDbContext _context;

        public AlgorithmService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Algorithm> AddAlgorithmAsync(AddAlgorithmDTO dto)
        {
            // Validate the name
            if (!Enum.TryParse(dto.Name, true, out AlgorithmName algorithmName))
            {
                throw new ArgumentException("Invalid algorithm name.");
            }

            var algorithm = new Algorithm
            {
                name = algorithmName.ToFriendlyString(),
                description = dto.Description,
                displayName = dto.DisplayName,
                requiresTimeQuantum = dto.RequiresTimeQuantum,
            };

            await _context.Algorithms.AddAsync(algorithm);
            await _context.SaveChangesAsync();

            return algorithm;
        }
    }

    public static class StaticAlgorithmService
    {
        public static void ExecuteFCFS(
            List<Process> processes,
            List<ExecutionLogEntry> executionLog
        )
        {
            // Sort processes by Arrival Time
            processes.Sort(
                (p1, p2) =>
                    p1.arrivalTime.GetValueOrDefault().CompareTo(p2.arrivalTime.GetValueOrDefault())
            );

            int currentTime = 0;

            foreach (var process in processes)
            {
                if (process.arrivalTime is null)
                    continue;

                // Wait if process hasn't arrived yet
                if (currentTime < process.arrivalTime)
                {
                    currentTime = process.arrivalTime.Value;
                }

                int startTime = currentTime;
                int endTime = currentTime + (process.burstTime ?? 0);

                // Record execution log
                executionLog.Add(
                    new ExecutionLogEntry
                    {
                        processId = process.id,
                        startTime = startTime,
                        endTime = endTime,
                    }
                );

                // Update process times
                process.completionTime = endTime;
                process.turnaroundTime = endTime - process.arrivalTime;
                process.waitingTime = process.turnaroundTime - process.burstTime;

                // Advance time
                currentTime = endTime;
            }
        }

        public static void ExecuteSJF(List<Process> processes, List<ExecutionLogEntry> executionLog)
        {
            int n = processes.Count;
            int completed = 0;
            int currentTime = 0;

            // Sort by arrival time initially
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            while (completed < n)
            {
                // Get all processes that have arrived but are not yet completed
                var availableProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.burstTime)
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    // No process available, move forward in time
                    currentTime++;
                }
                else
                {
                    var shortestJob = availableProcesses.First();

                    int startTime = currentTime;
                    int endTime = startTime + (shortestJob.burstTime ?? 0);

                    // Record execution log
                    executionLog.Add(
                        new ExecutionLogEntry
                        {
                            processId = shortestJob.id,
                            startTime = startTime,
                            endTime = endTime,
                        }
                    );

                    // Update process stats
                    shortestJob.completionTime = endTime;
                    shortestJob.turnaroundTime = endTime - shortestJob.arrivalTime;
                    shortestJob.waitingTime = shortestJob.turnaroundTime - shortestJob.burstTime;
                    shortestJob.isCompleted = true;

                    currentTime = endTime;
                    completed++;
                }
            }
        }

        public static void ExecuteSRTF(
            List<Process> processes,
            List<ExecutionLogEntry> executionLog
        )
        {
            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            foreach (var p in processes)
            {
                p.remainingTime = p.burstTime;
            }

            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            Process? lastProcess = null;
            int? executionStart = null;

            while (completed < n)
            {
                var availableProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.remainingTime)
                    .ThenBy(p => p.arrivalTime)
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    currentTime++;
                    continue;
                }

                var currentProcess = availableProcesses.First();

                // Start of a new execution segment
                if (lastProcess == null || currentProcess.id != lastProcess.id)
                {
                    // If last process was executing, close its segment
                    if (lastProcess != null && executionStart.HasValue)
                    {
                        executionLog.Add(
                            new ExecutionLogEntry
                            {
                                processId = lastProcess.id,
                                startTime = executionStart.Value,
                                endTime = currentTime,
                            }
                        );
                    }

                    // Start new segment
                    executionStart = currentTime;
                    lastProcess = currentProcess;
                }

                // Execute for 1 time unit
                currentProcess.remainingTime--;
                currentTime++;

                if (currentProcess.remainingTime == 0)
                {
                    currentProcess.isCompleted = true;
                    currentProcess.completionTime = currentTime;
                    currentProcess.turnaroundTime =
                        currentProcess.completionTime - currentProcess.arrivalTime;
                    currentProcess.waitingTime =
                        currentProcess.turnaroundTime - currentProcess.burstTime;
                    completed++;

                    // Close the current execution segment
                    if (executionStart.HasValue)
                    {
                        executionLog.Add(
                            new ExecutionLogEntry
                            {
                                processId = currentProcess.id,
                                startTime = executionStart.Value,
                                endTime = currentTime,
                            }
                        );

                        executionStart = null;
                        lastProcess = null;
                    }
                }
            }

            // Finalize any remaining open segment (if loop exits unexpectedly)
            if (lastProcess != null && executionStart.HasValue)
            {
                executionLog.Add(
                    new ExecutionLogEntry
                    {
                        processId = lastProcess.id,
                        startTime = executionStart.Value,
                        endTime = currentTime,
                    }
                );
            }
        }

        public static void RunPrioritySchedulingNonPreemptive(
            List<Process> processes,
            List<ExecutionLogEntry> executionLog
        )
        {
            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            // Sort by arrival time initially
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            while (completed < n)
            {
                // Get all processes that have arrived but are not yet completed
                var arrivedProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.priority) // lower number = higher priority
                    .ThenBy(p => p.arrivalTime)
                    .ToList();

                if (arrivedProcesses.Count == 0)
                {
                    currentTime++;
                }
                else
                {
                    var highestPriorityProcess = arrivedProcesses.First();

                    // Record execution in the log
                    executionLog.Add(
                        new ExecutionLogEntry
                        {
                            processId = highestPriorityProcess.id,
                            startTime = currentTime,
                            endTime = currentTime + (highestPriorityProcess.burstTime ?? 0),
                        }
                    );

                    // Update process times
                    highestPriorityProcess.completionTime =
                        currentTime + highestPriorityProcess.burstTime;
                    highestPriorityProcess.turnaroundTime =
                        highestPriorityProcess.completionTime - highestPriorityProcess.arrivalTime;
                    highestPriorityProcess.waitingTime =
                        highestPriorityProcess.turnaroundTime - highestPriorityProcess.burstTime;
                    highestPriorityProcess.isCompleted = true;

                    currentTime = highestPriorityProcess.completionTime.GetValueOrDefault();
                    completed++;
                }
            }
        }

        public static void RunPrioritySchedulingPreemptive(
            List<Process> processes,
            List<ExecutionLogEntry> executionLog
        )
        {
            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            foreach (var p in processes)
            {
                p.remainingTime = p.burstTime;
            }

            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            Process? lastProcess = null;
            int executionStart = -1;

            while (completed < n)
            {
                var arrivedProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.priority)
                    .ThenBy(p => p.arrivalTime)
                    .ToList();

                if (arrivedProcesses.Count == 0)
                {
                    // If no process is ready, close current running segment (if any)
                    if (lastProcess != null)
                    {
                        executionLog.Add(
                            new ExecutionLogEntry
                            {
                                processId = lastProcess.id,
                                startTime = executionStart,
                                endTime = currentTime,
                            }
                        );
                        lastProcess = null;
                    }

                    currentTime++;
                    continue;
                }

                var currentProcess = arrivedProcesses.First();

                // If the process being executed has changed, close the previous execution log
                if (lastProcess == null || lastProcess.id != currentProcess.id)
                {
                    if (lastProcess != null)
                    {
                        executionLog.Add(
                            new ExecutionLogEntry
                            {
                                processId = lastProcess.id,
                                startTime = executionStart,
                                endTime = currentTime,
                            }
                        );
                    }

                    lastProcess = currentProcess;
                    executionStart = currentTime;
                }

                currentProcess.remainingTime--;
                currentTime++;

                if (currentProcess.remainingTime == 0)
                {
                    currentProcess.isCompleted = true;
                    currentProcess.completionTime = currentTime;
                    currentProcess.turnaroundTime =
                        currentProcess.completionTime - currentProcess.arrivalTime;
                    currentProcess.waitingTime =
                        currentProcess.turnaroundTime - currentProcess.burstTime;
                    completed++;

                    // Finish the execution log for this process
                    executionLog.Add(
                        new ExecutionLogEntry
                        {
                            processId = currentProcess.id,
                            startTime = executionStart,
                            endTime = currentTime,
                        }
                    );

                    lastProcess = null;
                }
            }

            // In case the last process ends at the very end and isn't logged
            if (lastProcess != null && executionStart < currentTime)
            {
                executionLog.Add(
                    new ExecutionLogEntry
                    {
                        processId = lastProcess.id,
                        startTime = executionStart,
                        endTime = currentTime,
                    }
                );
            }
        }

        public static void RunRoundRobin(
            List<Process> processes,
            int timeQuantum,
            List<ExecutionLogEntry> executionLog
        )
        {
            // Sort processes by ArrivalTime
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            Queue<Process> readyQueue = new Queue<Process>();

            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            // Track arrived processes
            int index = 0;

            // Initialize remainingTime for all processes
            foreach (var process in processes)
            {
                process.remainingTime = process.burstTime;
            }

            while (completed < n)
            {
                // Enqueue processes that have arrived
                while (index < n && processes[index].arrivalTime <= currentTime)
                {
                    readyQueue.Enqueue(processes[index]);
                    index++;
                }

                if (readyQueue.Count == 0)
                {
                    // No process is ready, move to next arrival
                    if (index < n)
                    {
                        currentTime = processes[index].arrivalTime.GetValueOrDefault(currentTime);
                        continue;
                    }
                }
                else
                {
                    var currentProcess = readyQueue.Dequeue();

                    int timeSlice = Math.Min(
                        timeQuantum,
                        currentProcess.remainingTime.GetValueOrDefault()
                    );
                    int executionStart = currentTime;
                    currentTime += timeSlice;
                    currentProcess.remainingTime -= timeSlice;

                    // Log execution segment
                    executionLog.Add(
                        new ExecutionLogEntry
                        {
                            processId = currentProcess.id,
                            startTime = executionStart,
                            endTime = currentTime,
                        }
                    );

                    // Enqueue new arrivals during execution slice
                    while (index < n && processes[index].arrivalTime <= currentTime)
                    {
                        readyQueue.Enqueue(processes[index]);
                        index++;
                    }

                    if (currentProcess.remainingTime == 0)
                    {
                        currentProcess.isCompleted = true;
                        currentProcess.completionTime = currentTime;
                        currentProcess.turnaroundTime =
                            currentProcess.completionTime - currentProcess.arrivalTime;
                        currentProcess.waitingTime =
                            currentProcess.turnaroundTime - currentProcess.burstTime;
                        completed++;
                    }
                    else
                    {
                        // Not finished, re-queue it
                        readyQueue.Enqueue(currentProcess);
                    }
                }
            }
        }
    }
}
