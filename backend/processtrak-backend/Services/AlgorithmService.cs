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
        public static void ExecuteFCFS(List<Process> processes)
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

        public static void ExecuteSJF(List<Process> processes)
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

        public static void ExecuteSRTF(List<Process> processes)
        {
            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            // Initialize RemainingTime to BurstTime
            foreach (var p in processes)
            {
                p.remainingTime = p.burstTime;
            }

            // Sort initially by ArrivalTime (just to organize data, not strictly mandatory for SRTF)
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            while (completed < n)
            {
                // Select the process with the smallest remaining time among the arrived ones
                var availableProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.remainingTime)
                    .ThenBy(p => p.arrivalTime) // tie-breaker: earlier arrival
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    // No process has arrived yet, or all arrived processes are completed.
                    // Move time forward until the next process arrival.
                    currentTime++;
                    continue;
                }

                var currentProcess = availableProcesses.First(); // process with smallest remaining time

                // Run it for 1 time unit
                currentProcess.remainingTime--;
                currentTime++;

                // Check if completed
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
            }
        }

        public static void RunPrioritySchedulingNonPreemptive(List<Process> processes)
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
                    // Sort by priority ASC (smaller number = higher priority),
                    // tie-breaker by arrival time
                    .OrderBy(p => p.priority)
                    .ThenBy(p => p.arrivalTime)
                    .ToList();

                if (arrivedProcesses.Count == 0)
                {
                    // No process has arrived yet or all arrived processes have completed
                    // Move time forward until the next arrival
                    currentTime++;
                }
                else
                {
                    // Pick the process with the highest priority (lowest Priority value)
                    var highestPriorityProcess = arrivedProcesses.First();

                    // Run it to completion (non-preemptive)
                    highestPriorityProcess.completionTime =
                        currentTime + highestPriorityProcess.burstTime;
                    highestPriorityProcess.turnaroundTime =
                        highestPriorityProcess.completionTime - highestPriorityProcess.arrivalTime;
                    highestPriorityProcess.waitingTime =
                        highestPriorityProcess.turnaroundTime - highestPriorityProcess.burstTime;
                    highestPriorityProcess.isCompleted = true;

                    // Update currentTime
                    currentTime = highestPriorityProcess.completionTime.GetValueOrDefault();
                    completed++;
                }
            }
        }

        public static void RunPrioritySchedulingPreemptive(List<Process> processes)
        {
            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            // Initialize RemainingTime to BurstTime
            foreach (var p in processes)
            {
                p.remainingTime = p.burstTime;
            }

            // Sort primarily by arrival time to handle the earliest arrivals first
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            while (completed < n)
            {
                // 1) Get all processes that have arrived but not completed
                var arrivedProcesses = processes
                    .Where(p => p.arrivalTime <= currentTime && !p.isCompleted.GetValueOrDefault())
                    .OrderBy(p => p.priority) // smaller number = higher priority
                    .ThenBy(p => p.arrivalTime) // tie-breaker by arrival time
                    .ToList();

                // 2) If no process is available, move time forward
                if (arrivedProcesses.Count == 0)
                {
                    currentTime++;
                    continue;
                }

                // 3) Pick the highest priority (lowest Priority value) process
                var currentProcess = arrivedProcesses.First();

                // 4) Run it for 1 time unit
                currentProcess.remainingTime--;
                currentTime++;

                // 5) If the process has finished
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
            }
        }

        public static void RunRoundRobin(List<Process> processes, int timeQuantum)
        {
            // Sort processes by ArrivalTime (helps to manage them in order)
            processes = processes.OrderBy(p => p.arrivalTime).ToList();

            // Ready Queue to store process IDs or references
            Queue<Process> readyQueue = new Queue<Process>();

            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;

            // For convenience, index to track processes that have "arrived"
            int index = 0;

            // Keep running until all processes complete
            while (completed < n)
            {
                // Enqueue newly arrived processes whose arrival time <= currentTime
                while (index < n && processes[index].arrivalTime <= currentTime)
                {
                    readyQueue.Enqueue(processes[index]);
                    index++;
                }

                if (readyQueue.Count == 0)
                {
                    // No process is in the queue, so CPU is idle
                    // Move time to the arrival time of the next process
                    if (index < n && processes[index].arrivalTime > currentTime)
                    {
                        currentTime = processes[index].arrivalTime.GetValueOrDefault();
                        continue;
                    }
                }
                else
                {
                    // Get the front process
                    Process currentProcess = readyQueue.Dequeue();

                    // Run the process for 'timeQuantum' or until it finishes
                    int timeSlice = Math.Min(
                        timeQuantum,
                        currentProcess.remainingTime.GetValueOrDefault()
                    );

                    // Advance the current time by the time slice
                    currentTime += timeSlice;

                    // Decrease the remaining time of the process
                    currentProcess.remainingTime -= timeSlice;

                    // Again, enqueue newly arrived processes during this time slice
                    // Because a process might arrive while we're executing
                    while (index < n && processes[index].arrivalTime <= currentTime)
                    {
                        readyQueue.Enqueue(processes[index]);
                        index++;
                    }

                    // If the process is completed
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
                        // If it's not finished, place it back to the queue
                        readyQueue.Enqueue(currentProcess);
                    }
                }
            }
        }
    }
}
