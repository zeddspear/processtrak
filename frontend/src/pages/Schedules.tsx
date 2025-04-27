import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import {
  fetchSchedules,
  deleteSchedule,
  Schedule,
} from "../api/scheduleService";
import { FaTrash } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const Schedules = () => {
  const [error, setError] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const { data: schedules, isLoading } = useQuery({
    queryKey: ["schedules"],
    queryFn: fetchSchedules,
    retry: true,
  });

  const deleteScheduleMutation = useMutation({
    mutationFn: (id: string) => {
      return deleteSchedule(id);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["schedules"] });
    },
  });

  const handleDelete = async (id: string, e: React.MouseEvent) => {
    e.stopPropagation(); // Stop event propagation
    if (!confirm("Are you sure you want to delete this schedule?")) return;

    try {
      await deleteScheduleMutation.mutateAsync(id);
    } catch (err) {
      console.error(err);
      setError("Failed to delete schedule. Please try again.");
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800 dark:text-white">
          Schedule Management
        </h1>
      </div>

      {error && (
        <div
          className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4"
          role="alert"
        >
          <span className="block sm:inline">{error}</span>
        </div>
      )}

      {isLoading ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
        </div>
      ) : schedules?.length === 0 ? (
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 text-center">
          <p className="text-gray-600 dark:text-gray-400 text-lg">
            No schedules found.
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto bg-white dark:bg-gray-800 rounded-lg shadow-md">
          <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead className="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-bold text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  #
                </th>
                <th className="px-6 py-3 text-left text-xs font-bold text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Algorithm(s)
                </th>
                <th className="px-6 py-3 text-left text-xs font-bold text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Start Time
                </th>
                <th className="px-6 py-3 text-left text-xs font-bold text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  End Time
                </th>
                <th className="px-6 py-3 text-left text-xs font-bold text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Total Execution (ms)
                </th>
                <th className="px-6 py-3 text-left text-xs font-bold text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              {schedules?.map((schedule: Schedule, index: number) => {
                const algorithms = JSON.parse(schedule?.algorithmsJson) || [];

                return (
                  <tr
                    key={schedule.id}
                    onClick={() => navigate(`/schedules/${schedule.id}`)}
                    className="hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer transition-all duration-200"
                  >
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-200">
                      {index + 1}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                      <div className="flex gap-1 items-center">
                        {algorithms
                          ?.slice(0, 3)
                          .map((algo: any, idx: number) => (
                            <span
                              key={idx}
                              className="bg-blue-100 text-blue-800 text-xs font-semibold mr-1 px-2.5 py-0.5 rounded dark:bg-blue-900 dark:text-blue-300"
                            >
                              {algo.name}
                            </span>
                          ))}
                        {algorithms.length > 3 && (
                          <span
                            className="text-xs text-gray-500 dark:text-gray-400"
                            title={algorithms
                              ?.map((a: any) => a.name)
                              .join(", ")}
                          >
                            +{algorithms.length - 3} more
                          </span>
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                      {new Date(schedule.startTime).toLocaleString()}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                      {new Date(schedule.endTime).toLocaleString()}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                      {schedule.totalExecutionTime} ms
                    </td>
                    <td
                      className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200"
                      onClick={(e) => e.stopPropagation()} // Prevent row click from firing when clicking in actions cell
                    >
                      <button
                        className="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300 hover:cursor-pointer"
                        onClick={(e) => handleDelete(schedule.id, e)}
                      >
                        <FaTrash />
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default Schedules;
