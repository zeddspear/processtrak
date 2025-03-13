import { createBrowserRouter } from "react-router-dom";
import Home from "../pages/Home";
import About from "../pages/About";
import { ProtectedRoute } from "../components/ProtectedRoute";
import Layout from "../components/Layout";
import Login from "../pages/Login";
import Register from "../pages/Register";
import ForgotPassword from "../pages/ForgotPassword";
import ResetPassword from "../pages/ResetPassword";
import Processes from "../pages/Processes";
import Scheduling from "../pages/Scheduling";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <Login />,
  },
  {
    path: "/register",
    element: <Register />,
  },
  { path: "/forgot-password", element: <ForgotPassword /> },
  { path: "/reset-password", element: <ResetPassword /> },
  {
    path: "/",
    element: <Layout />,
    children: [
      {
        index: true,
        element: (
          <ProtectedRoute>
            <Home />
          </ProtectedRoute>
        ),
      },
      {
        path: "about",
        element: (
          <ProtectedRoute>
            <About />
          </ProtectedRoute>
        ),
      },
      {
        path: "processes",
        element: (
          <ProtectedRoute>
            <Processes />
          </ProtectedRoute>
        ),
      },
      {
        path: "scheduling",
        element: (
          <ProtectedRoute>
            <Scheduling />
          </ProtectedRoute>
        ),
      },
    ],
  },
]);
