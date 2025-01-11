namespace processtrak_backend.Utils
{
    public static class GlobalVariables
    {
        // Application-wide constants
        public static string JwtSecret => Environment.GetEnvironmentVariable("JWT_SECRET")!;

        public static int TokenExpirationHours => 2;

        // Application-wide settings (modifiable)
        public static string ApiBaseUrl { get; set; } = "http://localhost:5062";
    }
}
