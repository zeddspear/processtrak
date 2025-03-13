import { api } from "../lib/axios";

export interface Process {
  id: string;
  processId: string;
  name: string;
  arrivalTime: number;
  burstTime: number;
  priority: number | null;
  isCompleted: boolean;
}

export const fetchProcesses = async () => {
  const response = await api.get("/processes");

  console.log(response.data.$values);

  return response.data.$values;
};

export const createProcess = async (data: Partial<Process>) => {
  const response = await api.post("/processes", data);

  return response;
};

export const updateProcess = async (id: string, data: Partial<Process>) => {
  const response = await api.put(`/processes/${id}`, data);

  return response;
};

export const deleteProcess = async (id: string) => {
  const response = await api.delete(`/processes/${id}`);

  return response;
};
