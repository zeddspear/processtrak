using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AlgorithmsController(IAlgorithmService algorithmService)
        {
            _algorithmService = algorithmService;
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
            // Implementation for fetching algorithm by ID can go here.
            return Ok(); // Placeholder
        }
    }
}
