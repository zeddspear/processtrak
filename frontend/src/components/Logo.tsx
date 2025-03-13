const Logo = ({
  className = "",
  logo,
}: {
  className?: string;
  logo?: { width: string; height: string };
}) => {
  return (
    <div className={`flex items-center gap-3 ${className}`}>
      <div className="relative">
        {/* Outer Glow Effect */}
        <div className="absolute inset-0 bg-blue-500/20 dark:bg-blue-400/20 rounded-full blur-xl"></div>

        {/* Circular Icon */}
        <div
          className={`${logo?.width ? logo.width : "w-12"} ${
            logo?.height ? logo.height : "h-12"
          } bg-blue-600 dark:bg-blue-400 rounded-full relative z-10`}
        ></div>
      </div>

      {/* Logo Text */}
      <span className="text-3xl font-extrabold bg-gradient-to-r from-blue-600 to-blue-400 dark:from-blue-400 dark:to-blue-300 bg-clip-text text-transparent">
        ProcessTrak
      </span>
    </div>
  );
};

export default Logo;
