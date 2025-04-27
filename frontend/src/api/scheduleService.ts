import { api } from "../lib/axios";

export interface Process {
  id: string;
  processId: string;
  name: string;
  arrivalTime: number;
  burstTime: number;
  priority: number | null;
  isCompleted: boolean;
  completionTime?: number;
  turnaroundTime?: number;
  waitingTime?: number;
}

export interface GanttChartItem {
  processId: string;
  processName: string;
  startTime: number;
  endTime: number;
}

export const startScheduling = async (data: {
  processIds: string[];
  algorithmIds: string[];
  timeQuantum: number;
}) => {
  const response = await api.post("/processes/schedule/run", data);

  return response.data;
};

export interface Schedule {
  id: string;
  startTime: string; // ISO Date string
  endTime: string; // ISO Date string
  totalExecutionTime: number;
  averageWaitingTime: number;
  averageTurnaroundTime: number;
  algorithmsJson: string; // This is a stringified JSON array
  processesJson: string; // This is a stringified JSON array
  algorithms: {
    $id: string;
    $values: string[]; // Just array of algorithm names like ["srtf"]
  };
  processes: {
    $id: string;
    $values: any[]; // Empty array here, but you can define type later if needed
  };
}

export interface AlgorithmJson {
  name: string;
  description: string;
  displayName: string;
  requiresTimeQuantum: boolean;
  scheduleRuns: any[]; // You can type this better if needed
  id: string;
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
}

export interface ProcessJson {
  userId: string;
  processId: string;
  name: string;
  arrivalTime: number;
  burstTime: number;
  priority: number;
  remainingTime: number | null;
  startTime: string | null;
  completionTime: string | null;
  responseTime: number | null;
  turnaroundTime: number | null;
  waitingTime: number | null;
  isCompleted: boolean;
  State: number;
  user: any; // Assuming null, you can define it if needed
  id: string;
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
}

export const fetchSchedules = async (): Promise<Schedule[]> => {
  const response = await api.get("/processes/runs/get-all"); // Make sure your API route is correct
  return response.data.$values;
};

// In your scheduleService.ts or api file
export const deleteSchedule = async (id: string) => {
  const response = await api.delete(`/processes/runs/${id}`);

  return response.data;
};
