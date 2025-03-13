const Home = () => {
  return (
    <div className="container mx-auto px-4 py-12 text-center">
      <h1 className="text-5xl font-extrabold bg-gradient-to-r from-blue-500 to-blue-700 dark:from-blue-400 dark:to-blue-600 bg-clip-text text-transparent">
        Welcome to ProcessTrak
      </h1>
      <p className="text-lg text-gray-700 dark:text-gray-300 mt-4">
        Your one-stop solution for process management
      </p>

      <div className="mt-10 flex justify-center">
        <button className="px-6 py-3 bg-blue-600 dark:bg-blue-500 text-white text-lg font-medium rounded-md hover:bg-blue-700 dark:hover:bg-blue-600 transition">
          Get Started
        </button>
      </div>
    </div>
  );
};

export default Home;
