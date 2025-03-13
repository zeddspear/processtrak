using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.Models;

namespace processtrak_backend.Api.data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.Database.MigrateAsync(); // Ensure the database is created and migrated

            // Create the uuid-ossp extension if it doesn't exist
            await dbContext.Database.ExecuteSqlRawAsync(
                "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";"
            );

            // Seed the Algorithm table
            await SeedAlgorithmsAsync(dbContext);
        }

        private static async Task SeedAlgorithmsAsync(AppDbContext dbContext)
        {
            if (!await dbContext.Algorithms.AnyAsync())
            {
                var algorithms = new List<Algorithm>
                {
                    new Algorithm
                    {
                        id = Guid.Parse("2c6cb242-8053-462c-9bc4-f1e934a36b0c"),
                        name = "srtf",
                        description =
                            "In the Shortest Remaining Time First (SRTF) scheduling algorithm, the process with the smallest amount of time remaining until completion is selected to execute. Since the currently executing process is the one with the shortest amount of time remaining by definition, and since that time should only reduce as execution progresses, processes will always run until they complete or a new process is added that requires a smaller amount of time.",
                        displayName = "SRTF",
                        requiresTimeQuantum = false,
                    },
                    new Algorithm
                    {
                        id = Guid.Parse("bbece5a7-13c1-4688-92ee-e693a91926d8"),
                        name = "sjf",
                        description =
                            "The shortest job first (SJF) or shortest job next, is a scheduling policy that selects the waiting process with the smallest execution time to execute next. SJN, also known as Shortest Job Next (SJN), can be preemptive or non-preemptive.",
                        displayName = "SJF",
                        requiresTimeQuantum = false,
                    },
                };

                await dbContext.Algorithms.AddRangeAsync(algorithms);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
