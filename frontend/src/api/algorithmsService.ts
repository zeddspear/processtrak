import { api } from "../lib/axios";

export interface Algorithm {
  id: string;
  name: string;
  displayName: string;
  requiresTimeQuantum: boolean;
}

export const fetchAlgorithms = async () => {
  const response = await api.get("/algorithms");

  console.log(response.data.$values);

  return response.data.$values;
};
