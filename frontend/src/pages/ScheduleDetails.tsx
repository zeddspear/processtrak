import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import ScheduleStats from "../components/ScheduleStats";
import GanttChart from "../components/GanttChart";
import { fetchScheduleById } from "../api/scheduleService";

const ScheduleDetailsPage = () => {
  const { id } = useParams<{ id: string }>();

  const {
    data: schedule,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["schedule", id],
    queryFn: () => fetchScheduleById(id!),
    enabled: !!id,
  });

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
      </div>
    );
  }

  if (isError || !schedule) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="text-red-500">Failed to load schedule details</div>
      </div>
    );
  }

  const algorithms = JSON.parse(schedule.algorithmsJson);
  const processes = JSON.parse(schedule.processesJson);
  const executionLog = JSON.parse(schedule.executionLogJson);

  // Map for process metadata lookup
  const processMap = Object.fromEntries(processes.map((p: any) => [p.id, p]));

  const ganttItems = executionLog.map((log: any, index: number) => {
    const p = processMap[log.processId];

    return {
      id: `${log.processId}-${index}`, // unique id per segment
      key: `${log.processId}`,
      name: p.name,
      startValue: log.startTime,
      endValue: log.endTime,
      processId: p.processId,
      processName: p.name,
      arrivalTime: p.arrivalTime,
      burstTime: p.burstTime,
      priority: p.priority,
      endTime: p.completionTime,
    };
  });

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 dark:text-white mb-6">
        Schedule Details
      </h1>

      <div className="grid grid-cols-1 gap-8">
        {/* Schedule Metadata */}
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
            Schedule Information
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600 dark:text-gray-400">Start Time:</p>
              <p className="text-gray-800 dark:text-white">
                {new Date(schedule.startTime).toLocaleString()}
              </p>
            </div>
            <div>
              <p className="text-gray-600 dark:text-gray-400">End Time:</p>
              <p className="text-gray-800 dark:text-white">
                {new Date(schedule.endTime).toLocaleString()}
              </p>
            </div>
            <div>
              <p className="text-gray-600 dark:text-gray-400">Algorithm:</p>
              <p className="text-gray-800 dark:text-white">
                {algorithms[0]?.displayName || "Unknown"}
              </p>
            </div>
            <div>
              <p className="text-gray-600 dark:text-gray-400">
                Total Processes:
              </p>
              <p className="text-gray-800 dark:text-white">
                {processes.length}
              </p>
            </div>
          </div>
        </div>

        {/* Algorithm Information */}
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
            Algorithm Details
          </h2>
          <div className="space-y-2">
            <h3 className="font-semibold text-lg">
              {algorithms[0]?.displayName || "Unknown Algorithm"}
            </h3>
            <p className="text-gray-600 dark:text-gray-300">
              {algorithms[0]?.description || "No description available."}
            </p>
          </div>
        </div>

        {/* Results */}
        <ScheduleStats
          processes={processes}
          averageTurnaroundTime={schedule.averageTurnaroundTime}
          averageWaitingTime={schedule.averageWaitingTime}
          totalExecutionTime={schedule.totalExecutionTime}
        />

        {/* Gantt Chart */}
        {ganttItems.length > 0 && (
          <GanttChart
            items={ganttItems}
            title="Process Execution Timeline"
            timeFormat={(val) => `${val}ms`}
            customKeys={["burstTime", "priority"]}
            theme="dark"
          />
        )}
      </div>
    </div>
  );
};

export default ScheduleDetailsPage;
