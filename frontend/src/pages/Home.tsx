import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div className="container mx-auto px-4 py-12">
      <div className="text-center mb-12">
        <h1 className="text-5xl font-extrabold bg-gradient-to-r from-blue-500 to-blue-700 dark:from-blue-400 dark:to-blue-600 bg-clip-text text-transparent">
          Welcome to ProcessTrak
        </h1>
        <p className="text-lg text-gray-700 dark:text-gray-300 mt-4 max-w-2xl mx-auto">
          Your one-stop solution for process scheduling simulation. Add
          processes, select scheduling algorithms, and visualize the results
          with interactive Gantt charts.
        </p>
      </div>

      <div className="grid md:grid-cols-2 gap-8 max-w-4xl mx-auto">
        <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
          <h2 className="text-2xl font-bold text-blue-600 dark:text-blue-400 mb-4">
            Manage Processes
          </h2>
          <p className="text-gray-600 dark:text-gray-300 mb-6">
            Create, edit, and delete processes with custom parameters like
            arrival time, burst time, and priority.
          </p>
          <Link
            to="/processes"
            className="block text-center px-6 py-3 bg-blue-600 dark:bg-blue-500 text-white text-lg font-medium rounded-md hover:bg-blue-700 dark:hover:bg-blue-600 transition"
          >
            Manage Processes
          </Link>
        </div>

        <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
          <h2 className="text-2xl font-bold text-blue-600 dark:text-blue-400 mb-4">
            Run Scheduling
          </h2>
          <p className="text-gray-600 dark:text-gray-300 mb-6">
            Select from various scheduling algorithms like FCFS, SJF, Priority,
            and Round Robin to simulate process execution.
          </p>
          <Link
            to="/scheduling"
            className="block text-center px-6 py-3 bg-blue-600 dark:bg-blue-500 text-white text-lg font-medium rounded-md hover:bg-blue-700 dark:hover:bg-blue-600 transition"
          >
            Start Scheduling
          </Link>
        </div>

        {/* View Previous Schedules Box */}
        <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
          <h2 className="text-2xl font-bold text-blue-600 dark:text-blue-400 mb-4">
            View Previous Schedules
          </h2>
          <p className="text-gray-600 dark:text-gray-300 mb-6">
            Check out the list of all the schedules you have run previously to
            review your simulations and results.
          </p>
          <Link
            to="/previous-schedules" // Update the route to where you handle viewing previous schedules
            className="block text-center px-6 py-3 bg-blue-600 dark:bg-blue-500 text-white text-lg font-medium rounded-md hover:bg-blue-700 dark:hover:bg-blue-600 transition"
          >
            View Schedules
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Home;
