import { api } from "../lib/axios";

export interface LoginData {
  email: string;
  password: string;
}

export interface RegisterData {
  name: string;
  email: string;
  password: string;
  phone: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
  isGuest: boolean;
}

export const loginUser = async (data: LoginData) => {
  const response = await api.post<{ token: string; user: User }>(
    "/auth/login",
    data
  );
  localStorage.setItem("token", response.data.token); // Store token

  return await fetchCurrentUser();
};

export const loginAsGuestUser = async () => {
  const response = await api.post<{ token: string; user: User }>(
    "/auth/guest-login"
  );
  localStorage.setItem("token", response.data.token); // Store token

  return await fetchCurrentUser();
};

export const registerUser = async (data: RegisterData) => {
  const response = await api.post<{ user: User }>("/auth/register", data);
  return response.data.user;
};

export const fetchCurrentUser = async () => {
  const response = await api.get<{ user: User }>("/me");

  return response.data.user;
};

export const logoutUser = () => {
  localStorage.removeItem("token"); // Clear token
};

export const requestOTP = async (email: string) => {
  const response = await api.post("/auth/forgot-password", { email });

  // Save email to localStorage
  localStorage.setItem("resetEmail", response.data.email);

  return response.data;
};

export const verifyOTPAndResetPassword = async (data: {
  email: string;
  code: string;
  newPassword: string;
}) => {
  const response = await api.post("/auth/reset-password", data);

  // Clear email after successful password reset
  localStorage.removeItem("resetEmail");

  return response.data;
};
