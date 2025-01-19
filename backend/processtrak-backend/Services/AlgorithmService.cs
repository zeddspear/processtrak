using processtrak_backend.Api.data;
using processtrak_backend.Dto;
using processtrak_backend.Extensions;
using processtrak_backend.interfaces;
using processtrak_backend.Models;

namespace processtrak_backend.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly AppDbContext _context;

        public AlgorithmService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Algorithm> AddAlgorithmAsync(AddAlgorithmDTO dto)
        {
            // Validate the name
            if (!Enum.TryParse(dto.Name, true, out AlgorithmName algorithmName))
            {
                throw new ArgumentException("Invalid algorithm name.");
            }

            var algorithm = new Algorithm
            {
                name = algorithmName.ToFriendlyString(),
                description = dto.Description,
            };

            await _context.Algorithms.AddAsync(algorithm);
            await _context.SaveChangesAsync();

            return algorithm;
        }
    }
}
