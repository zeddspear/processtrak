import { useState } from "react";
import { FaClock, FaExclamationTriangle } from "react-icons/fa";
import {
  GanttChartItem,
  Process,
  startScheduling,
} from "../api/scheduleService";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { fetchProcesses } from "../api/processesService";
import { Algorithm, fetchAlgorithms } from "../api/algorithmsService";

const Scheduling = () => {
  const queryClient = useQueryClient();

  //   const [processes, setProcesses] = useState<Process[]>([]);
  const [selectedProcesses, setSelectedProcesses] = useState<string[]>([]);
  const [selectedAlgorithm, setSelectedAlgorithm] = useState<string>("");
  const [timeQuantum, setTimeQuantum] = useState<number>(1);
  const [error, setError] = useState<string | null>(null);
  const [results, setResults] = useState<Process[]>([]);
  const [ganttChart, setGanttChart] = useState<GanttChartItem[]>([]);
  const [averageMetrics, setAverageMetrics] = useState({
    turnaround: 0,
    waiting: 0,
    totalExecutionTime: 0,
  });

  // Fetching all processes
  const {
    data: fetchedProcesses,
    isLoading,
    isError: errorFetchingProcesses,
  } = useQuery({
    queryKey: ["processes"],
    queryFn: fetchProcesses,
    retry: true,
  });

  // Fetching all algorithms
  const {
    data: fetchedAlgorithms,
    isLoading: algorithmsIsLoading,
    isError: errorFetchingAlgorithms,
  } = useQuery({
    queryKey: ["algorithms"],
    queryFn: fetchAlgorithms,
    retry: true,
  });

  // Run schedule
  const runSchedulingMutation = useMutation({
    mutationFn: startScheduling,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["processes"] });
    },
  });

  const handleProcessSelection = (id: string) => {
    setSelectedProcesses((prevSelected) => {
      if (prevSelected.includes(id)) {
        return prevSelected.filter((pid) => pid !== id);
      } else {
        return [...prevSelected, id];
      }
    });
  };

  const handleSelectAll = () => {
    if (selectedProcesses.length === fetchedProcesses.length) {
      setSelectedProcesses([]);
    } else {
      setSelectedProcesses(fetchedProcesses.map((p: Process) => p.id));
    }
  };

  const handleAlgorithmChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedAlgorithm(e.target.value);
  };

  const handleTimeQuantumChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTimeQuantum(parseInt(e.target.value, 10));
  };

  const startSchedulingEvent = async () => {
    if (selectedProcesses.length === 0) {
      setError("Please select at least one process.");
      return;
    }

    if (!selectedAlgorithm) {
      setError("Please select a scheduling algorithm.");
      return;
    }

    const selectedAlgorithmObj = fetchedAlgorithms?.find(
      (a: Algorithm) => a.id === selectedAlgorithm
    );

    if (selectedAlgorithmObj?.requiresQuantum && timeQuantum <= 0) {
      setError("Please set a valid time quantum (greater than 0).");
      return;
    }

    try {
      setError(null);
      setResults([]);
      setGanttChart([]);

      const data = {
        processIds: selectedProcesses,
        algorithmIds: [selectedAlgorithm],
        timeQuantum: selectedAlgorithmObj?.requiresQuantum ? timeQuantum : 1,
      };

      const response: any = await runSchedulingMutation.mutateAsync(data);
      console.log("Schedule Response: ", response);

      // Process results from API
      // This is a placeholder - modify according to your actual API response
      if (response) {
        setResults(response.processes.$values);

        // Calculate average metrics
        const totalProcesses = response.processes.$values.length;
        if (totalProcesses > 0) {
          const avgTurnaround = response.averageTurnaroundTime;

          const avgWaiting = response.averageWaitingTime;

          setAverageMetrics({
            turnaround: avgTurnaround,
            waiting: avgWaiting,
            totalExecutionTime: response.totalExecutionTime,
          });
        }

        // Generate Gantt chart data from results

        // Fallback: Create basic Gantt chart from process data if API doesn't provide it
        const sortedProcesses = [...response.processes.$values].sort((a, b) => {
          if (a.startTime !== undefined && b.startTime !== undefined) {
            return a.startTime - b.startTime;
          }
          return 0;
        });

        const ganttItems: GanttChartItem[] = [];
        sortedProcesses.forEach((p) => {
          if (p.startTime !== undefined && p.completionTime !== undefined) {
            ganttItems.push({
              processId: p.processId,
              processName: p.name,
              startTime: p.startTime,
              endTime: p.completionTime,
            });
          }
        });

        setGanttChart(ganttItems);
      }

      setSelectedAlgorithm("");
      setSelectedProcesses([]);
    } catch (err) {
      console.error(err);
      setError("Failed to run scheduling algorithm. Please try again.");
    }
  };

  // Helper function to generate colors for processes
  const getProcessColor = (processId: string) => {
    // Generate a deterministic color based on the process ID
    const hash = processId.split("").reduce((acc, char) => {
      return char.charCodeAt(0) + ((acc << 5) - acc);
    }, 0);

    const h = Math.abs(hash % 360);
    return `hsl(${h}, 70%, 60%)`;
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 dark:text-white mb-6">
        Scheduling Simulation
      </h1>

      {error ||
        (errorFetchingProcesses && (
          <div
            className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4"
            role="alert"
          >
            <span className="block sm:inline">
              {error ||
                `${
                  errorFetchingProcesses &&
                  "Failed to load processes. Please try again."
                }` ||
                `${
                  errorFetchingAlgorithms &&
                  "Failed to load algorithms. Please try again."
                }`}
            </span>
          </div>
        ))}

      <div className="grid md:grid-cols-2 gap-8">
        <div>
          <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
            <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
              1. Select Processes
            </h2>

            {isLoading ? (
              <div className="flex justify-center items-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500"></div>
              </div>
            ) : fetchedProcesses.filter((p: Process) => !p.isCompleted)
                .length === 0 ? (
              <div className="text-center p-4">
                <FaExclamationTriangle className="mx-auto text-yellow-500 text-3xl mb-2" />
                <p className="text-gray-600 dark:text-gray-400">
                  No on going processes found. Please add processes first.
                </p>
              </div>
            ) : (
              <>
                <div className="flex justify-between items-center mb-2">
                  <button
                    onClick={handleSelectAll}
                    className="text-sm text-blue-600 dark:text-blue-400 hover:underline"
                  >
                    {selectedProcesses.length === fetchedProcesses.length
                      ? "Deselect All"
                      : "Select All"}
                  </button>
                  <span className="text-sm text-gray-500 dark:text-gray-400">
                    {selectedProcesses.length} of {fetchedProcesses.length}{" "}
                    selected
                  </span>
                </div>

                <div className="max-h-60 overflow-y-auto">
                  {fetchedProcesses.map((process: Process) => {
                    if (process.isCompleted) return;
                    return (
                      <div
                        key={process.id}
                        className="flex items-center p-2 border-b border-gray-200 dark:border-gray-700"
                      >
                        <input
                          type="checkbox"
                          id={`process-${process.id}`}
                          checked={selectedProcesses.includes(process.id)}
                          onChange={() => handleProcessSelection(process.id)}
                          className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                        />
                        <label
                          htmlFor={`process-${process.id}`}
                          className="ml-3 block text-gray-700 dark:text-gray-300"
                        >
                          {process.name} (ID: {process.processId}, Arrival:{" "}
                          {process.arrivalTime}, Burst: {process.burstTime}
                          {process.priority !== null
                            ? `, Priority: ${process.priority}`
                            : ""}
                          )
                        </label>
                      </div>
                    );
                  })}
                </div>
              </>
            )}
          </div>

          <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
              2. Select Algorithm
            </h2>
            {algorithmsIsLoading ? (
              <div className="flex justify-center items-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500"></div>
              </div>
            ) : (
              <div className="mb-4">
                <label
                  className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                  htmlFor="algorithm"
                >
                  Scheduling Algorithm
                </label>
                <select
                  id="algorithm"
                  value={selectedAlgorithm}
                  onChange={handleAlgorithmChange}
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                >
                  <option value="">Select an algorithm</option>
                  {fetchedAlgorithms?.map((algo: Algorithm) => (
                    <option key={algo.id} value={algo.id}>
                      {algo.displayName}
                    </option>
                  ))}
                </select>
              </div>
            )}

            {selectedAlgorithm &&
              fetchedAlgorithms.find(
                (a: Algorithm) => a.id === selectedAlgorithm
              )?.requiresTimeQuantum && (
                <div className="mb-4">
                  <label
                    className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                    htmlFor="timeQuantum"
                  >
                    Time Quantum
                  </label>
                  <div className="flex items-center">
                    <input
                      type="number"
                      id="timeQuantum"
                      value={timeQuantum}
                      onChange={handleTimeQuantumChange}
                      className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                      min="1"
                      required
                    />
                    <span className="ml-2 text-gray-600 dark:text-gray-400">
                      <FaClock />
                    </span>
                  </div>
                </div>
              )}

            {selectedAlgorithm && (
              <div className="mt-4 p-4 border border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 rounded-lg shadow">
                <h3 className="font-semibold text-gray-800 dark:text-gray-200">
                  Description:
                </h3>
                <p className="text-gray-600 dark:text-gray-300">
                  {fetchedAlgorithms.find(
                    (a: Algorithm) => a.id === selectedAlgorithm
                  )?.description || "No description available."}
                </p>
              </div>
            )}

            <button
              onClick={startSchedulingEvent}
              disabled={
                runSchedulingMutation.isPending ||
                selectedProcesses.length === 0 ||
                !selectedAlgorithm
              }
              className={`w-full mt-4 px-4 py-2 rounded-md text-white font-medium ${
                runSchedulingMutation.isPending ||
                selectedProcesses.length === 0 ||
                !selectedAlgorithm
                  ? "bg-gray-400 dark:bg-gray-600 cursor-not-allowed"
                  : "bg-blue-600 dark:bg-blue-500 hover:bg-blue-700 dark:hover:bg-blue-600"
              } transition`}
            >
              {runSchedulingMutation.isPending
                ? "Running..."
                : "Run Scheduling"}
            </button>
          </div>
        </div>

        <div>
          <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
            <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
              3. Results
            </h2>

            {results.length > 0 ? (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                  <thead className="bg-gray-50 dark:bg-gray-700">
                    <tr>
                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                        Process
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                        Arrival
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                        Burst
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                        Completion
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                        Turnaround
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                        Waiting
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                    {results.map((process) => (
                      <tr
                        key={process.id}
                        className="hover:bg-gray-50 dark:hover:bg-gray-700"
                      >
                        <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                          {process.name}
                        </td>
                        <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                          {process.arrivalTime}
                        </td>
                        <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                          {process.burstTime}
                        </td>
                        <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                          {process.completionTime}
                        </td>
                        <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                          {process.turnaroundTime}
                        </td>
                        <td className="px-3 py-2 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                          {process.waitingTime}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                <div className="mt-4 grid grid-cols-3 gap-4">
                  <div className="bg-blue-50 dark:bg-blue-900/20 p-3 rounded-lg">
                    <div className="text-xs text-blue-600 dark:text-blue-400 uppercase font-semibold">
                      Avg. Turnaround Time
                    </div>
                    <div className="mt-1 font-bold text-lg text-blue-800 dark:text-blue-300">
                      {averageMetrics.turnaround}
                    </div>
                  </div>
                  <div className="bg-green-50 dark:bg-green-900/20 p-3 rounded-lg">
                    <div className="text-xs text-green-600 dark:text-green-400 uppercase font-semibold">
                      Avg. Waiting Time
                    </div>
                    <div className="mt-1 font-bold text-lg text-green-800 dark:text-green-300">
                      {averageMetrics.waiting}
                    </div>
                  </div>
                  <div className="bg-purple-50 dark:bg-purple-900/20 p-3 rounded-lg">
                    <div className="text-xs text-purple-600 dark:text-purple-400 uppercase font-semibold">
                      Throughput
                    </div>
                    <div className="mt-1 font-bold text-lg text-purple-800 dark:text-purple-300">
                      {(
                        results.length /
                        Math.max(...results.map((p) => p.completionTime || 0))
                      ).toFixed(2)}{" "}
                      p/t
                    </div>
                  </div>
                </div>
              </div>
            ) : (
              <div className="text-center py-8 text-gray-500 dark:text-gray-400">
                Run the scheduling algorithm to see results
              </div>
            )}
          </div>

          {ganttChart.length > 0 && (
            <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
              <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
                Gantt Chart
              </h2>

              <div className="overflow-x-auto">
                <div className="min-w-full flex">
                  {ganttChart.map((item, index) => {
                    const width = (item.endTime - item.startTime) * 30; // Scale by 30px per time unit
                    return (
                      <div
                        key={index}
                        className="flex flex-col items-center border-r border-gray-300 dark:border-gray-600 relative"
                        style={{
                          width: `${width}px`,
                          minWidth: `${width}px`,
                          backgroundColor: getProcessColor(item.processId),
                        }}
                      >
                        <div className="px-2 py-1 text-xs text-gray-800 font-medium overflow-hidden whitespace-nowrap text-center w-full">
                          {item.processName}
                        </div>
                        <div className="absolute bottom-0 left-0 w-full text-center text-xs font-mono">
                          {item.startTime}
                        </div>
                        {index === ganttChart.length - 1 && (
                          <div className="absolute bottom-0 right-0 text-xs font-mono">
                            {item.endTime}
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>

                <div className="mt-6 border-t border-gray-300 dark:border-gray-600 pt-2">
                  <div className="flex flex-wrap gap-2">
                    {Array.from(
                      new Set(ganttChart.map((item) => item.processId))
                    ).map((processId) => {
                      const process = ganttChart.find(
                        (item) => item.processId === processId
                      );
                      return (
                        <div key={processId} className="flex items-center">
                          <div
                            className="w-4 h-4 rounded-sm mr-1"
                            style={{
                              backgroundColor: getProcessColor(processId),
                            }}
                          ></div>
                          <span className="text-xs text-gray-700 dark:text-gray-300">
                            {process?.processName || processId}
                          </span>
                        </div>
                      );
                    })}
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Scheduling;
