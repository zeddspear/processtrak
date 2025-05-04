import { createContext, useContext, ReactNode } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  loginUser,
  registerUser,
  fetchCurrentUser,
  logoutUser,
  User,
  LoginData,
  RegisterData,
  loginAsGuestUser,
} from "../api/authService";

interface AppContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (data: LoginData) => Promise<User>;
  loginAsGuest: () => Promise<User>;
  register: (data: RegisterData) => Promise<User>;
  logout: () => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

export const AppProvider = ({ children }: { children: ReactNode }) => {
  const queryClient = useQueryClient();

  // Fetch authenticated user
  const { data: user, isLoading } = useQuery({
    queryKey: ["currentUser"],
    queryFn: fetchCurrentUser,
    retry: false, // Don't retry if user is not authenticated
  });

  // Login Mutation
  const loginMutation = useMutation({
    mutationFn: loginUser,
    onSuccess: (user) => {
      queryClient.setQueryData(["currentUser"], user);
    },
  });

  const loginAsGuestMutation = useMutation({
    mutationFn: loginAsGuestUser,
    onSuccess: (user) => {
      queryClient.setQueryData(["currentUser"], user);
    },
  });

  // Register Mutation
  const registerMutation = useMutation({
    mutationFn: registerUser,
  });

  // Logout Function
  const logout = () => {
    logoutUser();
    queryClient.setQueryData(["currentUser"], null);
  };

  return (
    <AppContext.Provider
      value={{
        user: user || null,
        isAuthenticated: !!user,
        isLoading,
        login: async (data) => loginMutation.mutateAsync(data),
        loginAsGuest: async () => loginAsGuestMutation.mutateAsync(),
        register: async (data) => registerMutation.mutateAsync(data),
        logout,
      }}
    >
      {children}
    </AppContext.Provider>
  );
};

export const useApp = () => {
  const context = useContext(AppContext);
  if (context === undefined) {
    throw new Error("useApp must be used within an AppProvider");
  }
  return context;
};
