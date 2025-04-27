import React from "react";
import { Process } from "../api/scheduleService";

interface ScheduleStatsProps {
  processes: Process[];
  averageTurnaroundTime: number;
  averageWaitingTime: number;
  totalExecutionTime: number;
}

const ScheduleStats: React.FC<ScheduleStatsProps> = ({
  processes,
  averageTurnaroundTime,
  averageWaitingTime,
  totalExecutionTime,
}) => {
  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
      <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
        Results
      </h2>

      {processes.length > 0 ? (
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
              {processes.map((process) => (
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
                {averageTurnaroundTime.toFixed(2)}
              </div>
            </div>
            <div className="bg-green-50 dark:bg-green-900/20 p-3 rounded-lg">
              <div className="text-xs text-green-600 dark:text-green-400 uppercase font-semibold">
                Avg. Waiting Time
              </div>
              <div className="mt-1 font-bold text-lg text-green-800 dark:text-green-300">
                {averageWaitingTime.toFixed(2)}
              </div>
            </div>
            <div className="bg-purple-50 dark:bg-purple-900/20 p-3 rounded-lg">
              <div className="text-xs text-purple-600 dark:text-purple-400 uppercase font-semibold">
                Total Execution Time
              </div>
              <div className="mt-1 font-bold text-lg text-purple-800 dark:text-purple-300">
                {totalExecutionTime}
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="text-center py-8 text-gray-500 dark:text-gray-400">
          No results to display
        </div>
      )}
    </div>
  );
};

export default ScheduleStats;
