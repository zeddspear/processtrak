import { useNavigate } from "react-router-dom";
import { useApp } from "../context/AppContext";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import Logo from "../components/Logo";

const validationSchema = Yup.object({
  name: Yup.string()
    .min(2, "Name must be at least 2 characters")
    .required("Name is required"),
  email: Yup.string()
    .email("Invalid email address")
    .required("Email is required"),
  password: Yup.string()
    .min(8, "Password must be at least 8 characters")
    .matches(/[a-z]/, "Password must contain at least one lowercase letter")
    .matches(/[A-Z]/, "Password must contain at least one uppercase letter")
    .matches(/[0-9]/, "Password must contain at least one digit")
    .matches(/[\W_]/, "Password must contain at least one special character")
    .required("Password is required"),
  phone: Yup.string()
    .matches(/^\+?[1-9]\d{7,14}$/, "Enter a valid phone number")
    .required("Phone number is required"),
});

const Register = () => {
  const navigate = useNavigate();
  const { register } = useApp();

  return (
    <div className="min-h-screen flex items-center justify-center bg-amber-50 dark:bg-gray-900">
      <div className="w-full max-w-md bg-white dark:bg-gray-800 bg-opacity-50 backdrop-blur-md p-8 shadow-lg rounded-lg border border-gray-300 dark:border-gray-700">
        <div className="flex justify-center mb-6">
          <Logo logo={{ width: "w-8", height: "h-8" }} />
        </div>
        <h2 className="text-center text-3xl font-bold text-gray-900 dark:text-white">
          Create Account
        </h2>

        <Formik
          initialValues={{ name: "", email: "", password: "", phone: "" }}
          validationSchema={validationSchema}
          onSubmit={async (values, { setSubmitting }) => {
            try {
              await register(values);
              navigate("/login");
            } catch (error) {
              console.error("Registration failed", error);
            }
            setSubmitting(false);
          }}
        >
          {({ errors, touched, isSubmitting }) => (
            <Form className="space-y-4 mt-6">
              <Field
                name="name"
                type="text"
                className="input-field"
                placeholder="Full Name"
              />
              {errors.name && touched.name && (
                <p className="text-sm text-red-500">{errors.name}</p>
              )}

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

              <Field
                name="phone"
                type="tel"
                className="input-field"
                placeholder="Phone Number"
              />
              {errors.phone && touched.phone && (
                <p className="text-sm text-red-500">{errors.phone}</p>
              )}

              <button
                type="submit"
                disabled={isSubmitting}
                className="btn-primary"
              >
                {isSubmitting ? "Registering..." : "Register"}
              </button>
            </Form>
          )}
        </Formik>

        <p className="text-center text-gray-600 dark:text-gray-400 mt-4">
          Already have an account?{" "}
          <span
            className="text-blue-600 dark:text-blue-400 hover:underline cursor-pointer"
            onClick={() => navigate("/login")}
          >
            Sign In
          </span>
        </p>
      </div>
    </div>
  );
};

export default Register;
