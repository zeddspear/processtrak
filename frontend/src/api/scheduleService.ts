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
