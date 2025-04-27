import { useState } from "react";
import { FaClock, FaExclamationTriangle } from "react-icons/fa";
import { Process, startScheduling } from "../api/scheduleService";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { fetchProcesses } from "../api/processesService";
import { Algorithm, fetchAlgorithms } from "../api/algorithmsService";
import ScheduleStats from "../components/ScheduleStats";
import GanttChart, { GanttChartItem } from "../components/GanttChart";

const Scheduling = () => {
  const queryClient = useQueryClient();
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

      // Process results from API
      if (response) {
        // Use processesJson if available, otherwise fallback to $values
        const processesData = response.processesJson
          ? JSON.parse(response.processesJson)
          : response.processes?.$values || [];

        setResults(processesData);

        // Calculate average metrics
        const totalProcesses = processesData.length;
        if (totalProcesses > 0) {
          const avgTurnaround =
            response.averageTurnaroundTime ||
            processesData.reduce(
              (sum: number, p: Process) => sum + (p.turnaroundTime || 0),
              0
            ) / totalProcesses;

          const avgWaiting =
            response.averageWaitingTime ||
            processesData.reduce(
              (sum: number, p: Process) => sum + (p.waitingTime || 0),
              0
            ) / totalProcesses;

          const totalExecTime =
            response.totalExecutionTime ||
            Math.max(
              ...processesData.map((p: Process) => p.completionTime || 0)
            );

          setAverageMetrics({
            turnaround: avgTurnaround,
            waiting: avgWaiting,
            totalExecutionTime: totalExecTime,
          });

          // Generate Gantt chart data
          const ganttItems = processesData
            .filter((p: Process) => p.completionTime !== undefined)
            .map((p: Process) => ({
              id: p.processId.toString(),
              name: p.name,
              startValue: p.arrivalTime,
              endValue: p.completionTime,
              // Keep original properties for reference if needed
              processId: p.processId,
              processName: p.name,
              arrivalTime: p.arrivalTime,
              burstTime: p.burstTime,
              priority: p.priority,
              endTime: p.completionTime,
            }))
            .sort((a: any, b: any) => a.startValue! - b.startValue!);

          setGanttChart(ganttItems);
        }
      }

      setSelectedAlgorithm("");
      setSelectedProcesses([]);
    } catch (err) {
      console.error(err);
      setError("Failed to run scheduling algorithm. Please try again.");
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 dark:text-white mb-6">
        Scheduling Simulation
      </h1>

      {(error || errorFetchingProcesses || errorFetchingAlgorithms) && (
        <div
          className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4"
          role="alert"
        >
          <span className="block sm:inline">
            {error ||
              (errorFetchingProcesses &&
                "Failed to load processes. Please try again.") ||
              (errorFetchingAlgorithms &&
                "Failed to load algorithms. Please try again.")}
          </span>
        </div>
      )}

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
                  No ongoing processes found. Please add processes first.
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
                    if (process.isCompleted) return null;
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
          <ScheduleStats
            processes={results}
            averageTurnaroundTime={averageMetrics.turnaround}
            averageWaitingTime={averageMetrics.waiting}
            totalExecutionTime={averageMetrics.totalExecutionTime}
          />

          {ganttChart.length > 0 && (
            <GanttChart
              items={ganttChart}
              title="Process Scheduling"
              timeFormat={(val) => `${val}ms`}
            />
          )}
        </div>
      </div>
    </div>
  );
};

export default Scheduling;
