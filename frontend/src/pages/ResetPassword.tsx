import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import { useMutation, useQuery } from "@tanstack/react-query";
import * as Yup from "yup";
import { verifyOTPAndResetPassword } from "../api/authService";

const validationSchema = Yup.object({
  email: Yup.string()
    .email("Invalid email address")
    .required("Email is required"),
  code: Yup.string()
    .length(6, "OTP must be 6 digits")
    .required("OTP is required"),
  newPassword: Yup.string()
    .min(8, "Password must be at least 8 characters")
    .matches(/[a-z]/, "Password must contain at least one lowercase letter")
    .matches(/[A-Z]/, "Password must contain at least one uppercase letter")
    .matches(/[0-9]/, "Password must contain at least one digit")
    .matches(/[\W_]/, "Password must contain at least one special character")
    .required("New password is required"),
});

const ResetPassword = () => {
  const navigate = useNavigate();
  const [storedEmail, setStoredEmail] = useState("");

  // Fetch email from React Query cache (if available)
  const { data: cachedEmail } = useQuery({
    queryKey: ["resetEmail"],
    initialData: () => localStorage.getItem("resetEmail") || "", // Fallback to localStorage
  });

  useEffect(() => {
    if (cachedEmail) {
      setStoredEmail(cachedEmail);
    }
  }, [cachedEmail]);

  // React Query mutation for verifying OTP and resetting password
  const mutation = useMutation({
    mutationFn: verifyOTPAndResetPassword,
    onSuccess: () => {
      alert("Password successfully reset! Please login.");
      localStorage.removeItem("resetEmail"); // Clear email after success
      navigate("/login"); // Redirect on success
    },
    onError: () => {
      alert("Invalid OTP or email. Please try again.");
    },
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-amber-50 dark:bg-gray-900">
      <div className="w-full max-w-md bg-white dark:bg-gray-800 p-8 shadow-lg rounded-lg border border-gray-300 dark:border-gray-700">
        <h2 className="text-center text-3xl font-bold text-gray-900 dark:text-white">
          Reset Password
        </h2>
        <p className="text-center text-gray-600 dark:text-gray-400 mt-2">
          Enter your OTP and new password.
        </p>

        <Formik
          initialValues={{ email: storedEmail, code: "", newPassword: "" }}
          validationSchema={validationSchema}
          enableReinitialize // Reinitialize form when storedEmail updates
          onSubmit={(values) => mutation.mutate(values)}
        >
          {({}) => (
            <Form className="space-y-4 mt-6">
              {/* Auto-filled and disabled email field */}
              <Field
                name="email"
                type="email"
                className="input-field bg-gray-200 dark:bg-gray-700 cursor-not-allowed"
                disabled
              />

              <Field
                name="code"
                type="text"
                className="input-field"
                placeholder="Enter OTP"
              />
              <Field
                name="newPassword"
                type="password"
                className="input-field"
                placeholder="New Password"
              />

              <button
                type="submit"
                disabled={mutation.isPending}
                className="btn-primary"
              >
                {mutation.isPending ? "Resetting..." : "Reset Password"}
              </button>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
};

export default ResetPassword;
