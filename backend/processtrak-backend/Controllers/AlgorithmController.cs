using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.Dto;
using processtrak_backend.interfaces;

namespace processtrak_backend.Controllers
{
    [ApiController]
    [Route("api/algorithms")]
    [Authorize]
    public class AlgorithmsController : ControllerBase
    {
        private readonly IAlgorithmService _algorithmService;

        private readonly AppDbContext _context;

        public AlgorithmsController(IAlgorithmService algorithmService, AppDbContext context)
        {
            _algorithmService = algorithmService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddAlgorithm([FromBody] AddAlgorithmDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var algorithm = await _algorithmService.AddAlgorithmAsync(dto);
                return CreatedAtAction(
                    nameof(GetAlgorithmById),
                    new { id = algorithm.id },
                    algorithm
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Optional: Add a method to fetch a specific algorithm by ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAlgorithmById(Guid id)
        {
            var algo = await _context
                .Algorithms.Where((algo) => algo.id == id)
                .Select((algo) => new { algo.name, algo.description })
                .FirstOrDefaultAsync();

            if (algo == null)
            {
                return NotFound("Algorithm not found.");
            }

            return Ok(new { algo });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlgorithms()
        {
            var algorithms = await _context
                .Algorithms.Select(algo => new
                {
                    algo.id,
                    algo.name,
                    algo.description,
                })
                .ToListAsync();

            return Ok(algorithms);
        }
    }
}
