import { FaClock, FaChartLine } from "react-icons/fa";

const About = () => {
  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold mb-6 text-center text-gray-900 dark:text-white">
        About ProcessTrak
      </h1>
      <p className="text-gray-700 dark:text-gray-300 text-lg mb-8 text-center max-w-3xl mx-auto">
        ProcessTrak is a comprehensive solution designed to streamline process
        management and scheduling optimization.
      </p>

      <div className="grid md:grid-cols-2 gap-8 max-w-5xl mx-auto">
        <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md border border-gray-200 dark:border-gray-700">
          <FaClock className="text-4xl text-blue-500 dark:text-blue-400 mb-4" />
          <h2 className="text-xl font-semibold mb-2 text-gray-900 dark:text-white">
            Process Scheduling
          </h2>
          <p className="text-gray-700 dark:text-gray-300">
            Optimize your workflows with advanced scheduling algorithms.
          </p>
        </div>

        <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md border border-gray-200 dark:border-gray-700">
          <FaChartLine className="text-4xl text-blue-500 dark:text-blue-400 mb-4" />
          <h2 className="text-xl font-semibold mb-2 text-gray-900 dark:text-white">
            Performance Analytics
          </h2>
          <p className="text-gray-700 dark:text-gray-300">
            Track and analyze process performance with detailed insights.
          </p>
        </div>
      </div>
    </div>
  );
};

export default About;
