import { useState } from "react";
import { FaEdit, FaTrash, FaPlus } from "react-icons/fa";
import {
  createProcess,
  deleteProcess,
  fetchProcesses,
  Process,
  updateProcess,
} from "../api/processesService";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

const Processes = () => {
  const queryClient = useQueryClient();

  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [currentProcess, setCurrentProcess] = useState<Partial<Process> | null>(
    null
  );
  const [isEditing, setIsEditing] = useState(false);

  const { data: fetchedProcesses, isLoading } = useQuery({
    queryKey: ["processes"],
    queryFn: fetchProcesses,
    retry: true,
  });

  const createProcessMutation = useMutation({
    mutationFn: createProcess,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["processes"] });
    },
  });

  const updateProcessMutation = useMutation({
    mutationFn: (process: Partial<Process>) => {
      return updateProcess(process.id!, process);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["processes"] });
    },
  });

  const deleteProcessMutation = useMutation({
    mutationFn: (id: string) => {
      return deleteProcess(id!);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["processes"] });
    },
  });

  const handleOpenModal = (process?: Process) => {
    if (process) {
      setCurrentProcess(process);
      setIsEditing(true);
    } else {
      setCurrentProcess({
        processId: "",
        name: "",
        arrivalTime: 0,
        burstTime: 0,
        priority: 0,
      });
      setIsEditing(false);
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setCurrentProcess(null);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setCurrentProcess((prev) => ({
      ...prev,
      [name]:
        name === "name" || name === "processId" ? value : parseInt(value, 10),
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!currentProcess) return;

    let response = null;

    try {
      if (isEditing) {
        response = await updateProcessMutation.mutateAsync(currentProcess);
      } else {
        response = await createProcessMutation.mutateAsync(currentProcess);
      }

      handleCloseModal();
    } catch (err) {
      if (createProcessMutation.isError || updateProcessMutation.isError) {
        setError(
          `Failed to ${
            isEditing ? "update" : "create"
          } process. Please try again.`
        );
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this process?")) return;

    try {
      await deleteProcessMutation.mutateAsync(id);
    } catch (err) {
      console.error(err);
      setError("Failed to delete process. Please try again.");
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800 dark:text-white">
          Process Management
        </h1>
        <button
          onClick={() => handleOpenModal()}
          className="px-4 py-2 bg-blue-600 dark:bg-blue-500 text-white rounded-md hover:bg-blue-700 dark:hover:bg-blue-600 transition flex items-center"
        >
          <FaPlus className="mr-2" /> Add Process
        </button>
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
      ) : fetchedProcesses.length === 0 ? (
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 text-center">
          <p className="text-gray-600 dark:text-gray-400 text-lg">
            No processes found. Add your first process to get started!
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto bg-white dark:bg-gray-800 rounded-lg shadow-md">
          <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead className="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Process ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Name
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Arrival Time
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Burst Time
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Priority
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Completed
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              {fetchedProcesses.map((process: Process) => (
                <tr
                  key={process.id}
                  className="hover:bg-gray-50 dark:hover:bg-gray-700"
                >
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.processId}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.arrivalTime}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.burstTime}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.priority ?? "N/A"}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.isCompleted ? "Yes" : "No"}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                    <button
                      onClick={() => handleOpenModal(process)}
                      className="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300 mr-3"
                    >
                      <FaEdit />
                    </button>
                    <button
                      onClick={() => handleDelete(process.id)}
                      className="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300"
                    >
                      <FaTrash />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Add/Edit Process Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex justify-center items-center p-4">
          <div className="bg-white dark:bg-gray-800 rounded-lg w-full max-w-md">
            <div className="p-6">
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                {isEditing ? "Edit Process" : "Add New Process"}
              </h2>
              <form onSubmit={handleSubmit}>
                <div className="mb-4">
                  <label
                    className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                    htmlFor="processId"
                  >
                    Process ID
                  </label>
                  <input
                    type="text"
                    id="processId"
                    name="processId"
                    disabled={isEditing}
                    value={currentProcess?.processId || ""}
                    onChange={handleInputChange}
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    required
                  />
                </div>
                <div className="mb-4">
                  <label
                    className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                    htmlFor="name"
                  >
                    Name
                  </label>
                  <input
                    type="text"
                    id="name"
                    name="name"
                    disabled={isEditing}
                    value={currentProcess?.name || ""}
                    onChange={handleInputChange}
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    required
                  />
                </div>
                <div className="mb-4">
                  <label
                    className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                    htmlFor="arrivalTime"
                  >
                    Arrival Time
                  </label>
                  <input
                    type="number"
                    id="arrivalTime"
                    name="arrivalTime"
                    value={currentProcess?.arrivalTime || 0}
                    onChange={handleInputChange}
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    min="0"
                    required
                  />
                </div>
                <div className="mb-4">
                  <label
                    className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                    htmlFor="burstTime"
                  >
                    Burst Time
                  </label>
                  <input
                    type="number"
                    id="burstTime"
                    name="burstTime"
                    value={currentProcess?.burstTime || 0}
                    onChange={handleInputChange}
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    min="1"
                    required
                  />
                </div>
                <div className="mb-6">
                  <label
                    className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                    htmlFor="priority"
                  >
                    Priority
                  </label>
                  <input
                    type="number"
                    id="priority"
                    name="priority"
                    value={currentProcess?.priority || 0}
                    onChange={handleInputChange}
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    min="0"
                  />
                </div>
                <div className="flex justify-end">
                  <button
                    type="button"
                    onClick={handleCloseModal}
                    className="bg-gray-300 dark:bg-gray-600 text-gray-800 dark:text-gray-200 py-2 px-4 rounded mr-2 hover:bg-gray-400 dark:hover:bg-gray-500 transition"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={createProcessMutation.isPending}
                    className="bg-blue-600 dark:bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-700 dark:hover:bg-blue-600 transition disabled:bg-gray-500 disabled:cursor-not-allowed"
                  >
                    {isEditing ? "Update" : "Create"}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Processes;
