import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Algorithm, fetchAlgorithms } from "../api/algorithmsService";
import { fetchProcesses } from "../api/processesService";
import {
  AlgorithmJson,
  Process,
  Schedule,
  reRunScheduling,
  runScheduling,
} from "../api/scheduleService";
import { FaClock, FaExclamationTriangle } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const EditScheduleModal = ({
  schedule,
  onClose,
}: {
  schedule: Schedule;
  onClose: () => void;
}) => {
  const queryClient = useQueryClient();

  const navigate = useNavigate();

  const [selectedProcesses, setSelectedProcesses] = useState<string[]>([]);
  const [selectedAlgorithm, setSelectedAlgorithm] = useState<string>("");
  const [timeQuantum, setTimeQuantum] = useState<number>(1);
  const [error, setError] = useState<string | null>(null);

  const { data: algorithms = [], isLoading: algorithmsIsLoading } = useQuery({
    queryKey: ["algorithms"],
    queryFn: fetchAlgorithms,
  });

  const { data: processes = [], isLoading: processesIsLoading } = useQuery({
    queryKey: ["processes"],
    queryFn: fetchProcesses,
  });

  const mutation = useMutation({
    mutationFn: ({
      scheduleId,
      data,
    }: {
      scheduleId: string;
      data: {
        processIds: string[];
        algorithmIds: string[];
        timeQuantum: number;
      };
    }) => reRunScheduling(scheduleId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["schedules"] });
      onClose();
    },
  });

  const handleProcessSelection = (id: string) => {
    setSelectedProcesses((prev) =>
      prev.includes(id) ? prev.filter((pid) => pid !== id) : [...prev, id]
    );
  };

  const handleSelectAll = () => {
    if (selectedProcesses.length === processes.length) {
      setSelectedProcesses([]);
    } else {
      setSelectedProcesses(processes.map((p: Process) => p.id));
    }
  };

  const handleSubmit = async () => {
    if (!selectedProcesses.length || !selectedAlgorithm) {
      setError("Please select both an algorithm and at least one process.");
      return;
    }

    setError(null);

    try {
      const response = await mutation.mutateAsync({
        scheduleId: schedule.id,
        data: {
          processIds: selectedProcesses,
          algorithmIds: [selectedAlgorithm],
          timeQuantum,
        },
      });

      console.log("Reeeee: ", response);

      navigate(`/schedules/${schedule.id}`);
    } catch (error) {
      setError("An error occurred while submitting the schedule.");
      console.error(error);
    }
  };

  const selectedAlgo = algorithms.find(
    (a: Algorithm) => a.id === selectedAlgorithm
  );

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-30 px-4">
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-lg w-full max-w-2xl p-6 max-h-[90vh] overflow-y-auto">
        <h2 className="text-2xl font-semibold text-gray-800 dark:text-white mb-6">
          Edit & Rerun Schedule
        </h2>

        {/* Error */}
        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 p-3 mb-4 rounded">
            {error}
          </div>
        )}

        {/* Processes Section */}
        <div className="mb-6">
          <h3 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
            1. Select Processes
          </h3>

          {processesIsLoading ? (
            <div className="flex justify-center items-center h-24">
              <div className="animate-spin h-6 w-6 rounded-full border-t-2 border-b-2 border-blue-600" />
            </div>
          ) : processes.length === 0 ? (
            <div className="text-center text-gray-600 dark:text-gray-400">
              <FaExclamationTriangle className="text-yellow-500 mx-auto text-2xl mb-2" />
              No processes found. Please add processes first.
            </div>
          ) : (
            <>
              <div className="flex justify-between mb-2">
                <button
                  onClick={handleSelectAll}
                  className="text-sm text-blue-600 dark:text-blue-400 hover:underline"
                >
                  {selectedProcesses.length === processes.length
                    ? "Deselect All"
                    : "Select All"}
                </button>
                <span className="text-sm text-gray-500 dark:text-gray-400">
                  {selectedProcesses.length} of {processes.length} selected
                </span>
              </div>
              <div className="max-h-48 overflow-y-auto border rounded p-2">
                {processes.map((p: Process) => (
                  <label
                    key={p.id}
                    className="flex items-center space-x-2 py-1"
                  >
                    <input
                      type="checkbox"
                      checked={selectedProcesses.includes(p.id)}
                      onChange={() => handleProcessSelection(p.id)}
                    />
                    <span className="text-gray-700 dark:text-gray-300">
                      {p.name} (Arrival: {p.arrivalTime}, Burst: {p.burstTime}
                      {p.priority !== null ? `, Priority: ${p.priority}` : ""})
                    </span>
                  </label>
                ))}
              </div>
            </>
          )}
        </div>

        {/* Algorithm Section */}
        <div className="mb-6">
          <h3 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
            2. Select Algorithm
          </h3>

          {algorithmsIsLoading ? (
            <div className="flex justify-center items-center h-24">
              <div className="animate-spin h-6 w-6 rounded-full border-t-2 border-b-2 border-blue-600" />
            </div>
          ) : (
            <>
              <select
                value={selectedAlgorithm}
                onChange={(e) => setSelectedAlgorithm(e.target.value)}
                className="w-full mb-4 p-2 rounded border dark:bg-gray-700 dark:text-white"
              >
                <option value="">-- Select Algorithm --</option>
                {algorithms.map((algo: AlgorithmJson) => (
                  <option key={algo.id} value={algo.id}>
                    {algo.displayName}
                  </option>
                ))}
              </select>

              {selectedAlgo?.requiresTimeQuantum && (
                <div className="mb-4">
                  <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    Time Quantum
                  </label>
                  <div className="flex items-center gap-2">
                    <input
                      type="number"
                      value={timeQuantum}
                      onChange={(e) => setTimeQuantum(Number(e.target.value))}
                      className="w-full p-2 border rounded dark:bg-gray-700 dark:text-white"
                      min={1}
                    />
                    <FaClock className="text-gray-500 dark:text-gray-300" />
                  </div>
                </div>
              )}

              {selectedAlgo?.description && (
                <div className="bg-gray-100 dark:bg-gray-700 p-4 rounded shadow-inner">
                  <h4 className="font-semibold text-gray-700 dark:text-gray-200 mb-1">
                    Description:
                  </h4>
                  <p className="text-gray-600 dark:text-gray-300 text-sm">
                    {selectedAlgo.description}
                  </p>
                </div>
              )}
            </>
          )}
        </div>

        {/* Footer Buttons */}
        <div className="flex justify-end gap-3 mt-4">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-white rounded hover:bg-gray-300 dark:hover:bg-gray-600"
          >
            Cancel
          </button>
          <button
            onClick={handleSubmit}
            disabled={
              mutation.isPending ||
              !selectedProcesses.length ||
              !selectedAlgorithm
            }
            className={`px-4 py-2 rounded text-white font-medium ${
              mutation.isPending ||
              !selectedProcesses.length ||
              !selectedAlgorithm
                ? "bg-gray-400 dark:bg-gray-600 cursor-not-allowed"
                : "bg-blue-600 hover:bg-blue-700"
            }`}
          >
            {mutation.isPending ? "Updating..." : "Update & Rerun"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditScheduleModal;
