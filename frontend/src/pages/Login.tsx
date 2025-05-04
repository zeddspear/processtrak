import { useNavigate } from "react-router-dom";
import { useApp } from "../context/AppContext";
import { Formik, Form, Field } from "formik";
import Logo from "../components/Logo";
import * as Yup from "yup";

const validationSchema = Yup.object({
  email: Yup.string()
    .email("Invalid email address")
    .required("Email is required"),
  password: Yup.string().required("Password is required"),
});

const Login = () => {
  const navigate = useNavigate();
  const { login, loginAsGuest } = useApp();

  return (
    <div className="min-h-screen flex items-center justify-center bg-amber-50 dark:bg-gray-900">
      <div className="w-full max-w-md bg-white dark:bg-gray-800 bg-opacity-50 backdrop-blur-md p-8 shadow-lg rounded-lg border border-gray-300 dark:border-gray-700">
        <div className="flex justify-center mb-6">
          <Logo logo={{ width: "w-8", height: "h-8" }} />
        </div>
        <h2 className="text-center text-3xl font-bold text-gray-900 dark:text-white">
          Sign In
        </h2>

        <Formik
          initialValues={{ email: "", password: "" }}
          validationSchema={validationSchema}
          onSubmit={async (values, { setSubmitting, setErrors }) => {
            try {
              await login(values);
              navigate("/");
            } catch (error: any) {
              console.error("Login failed", error);
              if (error.response?.data) {
                setErrors({ password: error.response.data });
              }
            }
            setSubmitting(false);
          }}
        >
          {({ errors, touched, isSubmitting }) => (
            <>
              <Form className="space-y-4 mt-6">
                <Field
                  name="email"
                  type="email"
                  className="input-field"
                  placeholder="Email Address"
                />
                {errors.email && touched.email && (
                  <p className="text-sm text-red-500">{errors.email}</p>
                )}

                <Field
                  name="password"
                  type="password"
                  className="input-field"
                  placeholder="Password"
                />
                {errors.password && touched.password && (
                  <p className="text-sm text-red-500">{errors.password}</p>
                )}

                {/* Forgot Password Link */}
                <div className="text-right">
                  <span
                    className="text-sm text-blue-600 dark:text-blue-400 hover:underline cursor-pointer"
                    onClick={() => navigate("/forgot-password")}
                  >
                    Forgot Password?
                  </span>
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="btn-primary"
                >
                  {isSubmitting ? "Signing in..." : "Sign In"}
                </button>
              </Form>
              {/* Guest Sign In Button */}
              <div className="mt-4">
                <button
                  type="button"
                  className="btn-secondary w-full"
                  onClick={async () => {
                    try {
                      await loginAsGuest();
                      navigate("/");
                    } catch (error) {
                      console.error("Guest login failed", error);
                    }
                  }}
                >
                  Sign In as Guest
                </button>
              </div>
            </>
          )}
        </Formik>

        <p className="text-center text-gray-600 dark:text-gray-400 mt-4">
          Don't have an account?{" "}
          <span
            className="text-blue-600 dark:text-blue-400 hover:underline cursor-pointer"
            onClick={() => navigate("/register")}
          >
            Sign Up
          </span>
        </p>
      </div>
    </div>
  );
};

export default Login;
