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
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";

const validationSchema = Yup.object({
  processId: Yup.string().required("Process ID is required"),
  name: Yup.string().required("Name is required"),
  arrivalTime: Yup.number()
    .min(0, "Arrival time must be at least 0")
    .required("Arrival time is required"),
  burstTime: Yup.number()
    .min(1, "Burst time must be at least 1")
    .required("Burst time is required"),
  priority: Yup.number()
    .min(1, "Priority must be at least 1")
    .required("Priority is required"),
});

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
      return deleteProcess(id);
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
        priority: 1, // set default priority to 1
      });
      setIsEditing(false);
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setCurrentProcess(null);
  };

  const handleSubmit = async (
    values: Partial<Process>,
    { setSubmitting }: { setSubmitting: (isSubmitting: boolean) => void }
  ) => {
    let response = null;
    try {
      if (isEditing) {
        response = await updateProcessMutation.mutateAsync({
          id: currentProcess?.id,
          ...values,
        });
      } else {
        response = await createProcessMutation.mutateAsync(values);
      }
      handleCloseModal();
    } catch (err) {
      console.log("Error happened: ", err);
      setError(
        `Failed to ${
          isEditing ? "update" : "create"
        } process. Please try again.`
      );
    }
    setSubmitting(false);
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
                {/* <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Completed
                </th> */}
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
                    {process.priority}
                  </td>
                  {/* <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-200">
                    {process.isCompleted ? "Yes" : "No"}
                  </td> */}
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                    {/* {!process.isCompleted && ( */}
                    <button
                      onClick={() => handleOpenModal(process)}
                      className="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300 mr-3 hover:cursor-pointer"
                    >
                      <FaEdit />
                    </button>
                    {/* )} */}

                    <button
                      onClick={() => handleDelete(process.id)}
                      className="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300 hover:cursor-pointer"
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
          <div className="bg-white dark:bg-gray-800 rounded-lg w-full max-w-md shadow-lg">
            <div className="p-6">
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                {isEditing ? "Edit Process" : "Add New Process"}
              </h2>
              <Formik
                initialValues={{
                  processId: currentProcess?.processId || "",
                  name: currentProcess?.name || "",
                  arrivalTime: currentProcess?.arrivalTime || 0,
                  burstTime: currentProcess?.burstTime || 1,
                  priority: currentProcess?.priority || 1,
                }}
                validationSchema={validationSchema}
                onSubmit={handleSubmit}
              >
                {({ errors, touched, isSubmitting }) => (
                  <Form className="space-y-4">
                    <div className="mb-4">
                      <label
                        className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                        htmlFor="processId"
                      >
                        Process ID
                      </label>
                      <Field
                        name="processId"
                        type="text"
                        className="shadow-sm border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition"
                        required
                      />
                      {errors.processId && touched.processId && (
                        <p className="text-sm text-red-500">
                          {errors.processId}
                        </p>
                      )}
                    </div>

                    <div className="mb-4">
                      <label
                        className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                        htmlFor="name"
                      >
                        Name
                      </label>
                      <Field
                        name="name"
                        type="text"
                        className="shadow-sm border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition"
                        required
                      />
                      {errors.name && touched.name && (
                        <p className="text-sm text-red-500">{errors.name}</p>
                      )}
                    </div>

                    <div className="mb-4">
                      <label
                        className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                        htmlFor="arrivalTime"
                      >
                        Arrival Time
                      </label>
                      <Field
                        name="arrivalTime"
                        type="number"
                        className="shadow-sm border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition"
                        min="0"
                        required
                      />
                      {errors.arrivalTime && touched.arrivalTime && (
                        <p className="text-sm text-red-500">
                          {errors.arrivalTime}
                        </p>
                      )}
                    </div>

                    <div className="mb-4">
                      <label
                        className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                        htmlFor="burstTime"
                      >
                        Burst Time
                      </label>
                      <Field
                        name="burstTime"
                        type="number"
                        className="shadow-sm border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition"
                        min="1"
                        required
                      />
                      {errors.burstTime && touched.burstTime && (
                        <p className="text-sm text-red-500">
                          {errors.burstTime}
                        </p>
                      )}
                    </div>

                    <div className="mb-6">
                      <label
                        className="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
                        htmlFor="priority"
                      >
                        Priority
                      </label>
                      <Field
                        name="priority"
                        type="number"
                        className="shadow-sm border rounded w-full py-2 px-3 text-gray-700 dark:text-gray-300 dark:bg-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition"
                        min="1"
                        required
                      />
                      {errors.priority && touched.priority && (
                        <p className="text-sm text-red-500">
                          {errors.priority}
                        </p>
                      )}
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
                        disabled={isSubmitting}
                        className="bg-blue-600 dark:bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-700 dark:hover:bg-blue-600 transition disabled:bg-gray-500 disabled:cursor-not-allowed"
                      >
                        {isSubmitting
                          ? "Saving..."
                          : isEditing
                          ? "Update"
                          : "Create"}
                      </button>
                    </div>
                  </Form>
                )}
              </Formik>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Processes;
