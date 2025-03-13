import { useNavigate } from "react-router-dom";
import { Formik, Form, Field } from "formik";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import * as Yup from "yup";
import { requestOTP } from "../api/authService";

const validationSchema = Yup.object({
  email: Yup.string()
    .email("Invalid email address")
    .required("Email is required"),
});

const ForgotPassword = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  // React Query mutation for sending OTP
  const mutation = useMutation({
    mutationFn: requestOTP,
    onSuccess: (data) => {
      // Cache email in React Query
      queryClient.setQueryData(["resetEmail"], data.email);
      navigate("/reset-password"); // Redirect on success
    },
    onError: () => {
      alert("Failed to send OTP. Please try again.");
    },
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-amber-50 dark:bg-gray-900">
      <div className="w-full max-w-md bg-white dark:bg-gray-800 p-8 shadow-lg rounded-lg border border-gray-300 dark:border-gray-700">
        <h2 className="text-center text-3xl font-bold text-gray-900 dark:text-white">
          Forgot Password
        </h2>
        <p className="text-center text-gray-600 dark:text-gray-400 mt-2">
          Enter your email to receive an OTP.
        </p>

        <Formik
          initialValues={{ email: "" }}
          validationSchema={validationSchema}
          onSubmit={(values) => mutation.mutate(values.email)}
        >
          {({ isSubmitting }) => (
            <Form className="space-y-4 mt-6">
              <Field
                name="email"
                type="email"
                className="input-field"
                placeholder="Email Address"
              />
              <button
                type="submit"
                disabled={mutation.isPending}
                className="btn-primary"
              >
                {mutation.isPending ? "Sending OTP..." : "Send OTP"}
              </button>
            </Form>
          )}
        </Formik>
      </div>
    </div>
  );
};

export default ForgotPassword;
