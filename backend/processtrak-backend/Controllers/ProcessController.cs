using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using processtrak_backend.Dto;
using processtrak_backend.interfaces;

[ApiController]
[Route("api/processes")]
[Authorize]
public class ProcessesController : ControllerBase
{
    private readonly IProcessService _processService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProcessesController(
        IProcessService processService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _processService = processService;
        _httpContextAccessor = httpContextAccessor;
    }

    // Helper to get the current user ID
    private Guid GetUserId() =>
        Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid)!);

    // 1. Get All Processes
    [HttpGet]
    public async Task<IActionResult> GetAllProcesses()
    {
        var userId = GetUserId();
        var processes = await _processService.GetAllProcessesAsync(userId);
        return Ok(processes);
    }

    // 2. Get Process By ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProcessById(Guid id)
    {
        var userId = GetUserId();
        var process = await _processService.GetProcessByIdAsync(userId, id);

        if (process == null)
            return NotFound(new { Message = "Process not found" });

        return Ok(process);
    }

    // 3. Create Process
    [HttpPost]
    public async Task<IActionResult> CreateProcess([FromBody] CreateProcessDTO dto)
    {
        var userId = GetUserId();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var process = await _processService.CreateProcessAsync(userId, dto);
        return CreatedAtAction(nameof(GetProcessById), new { id = process.id }, process);
    }

    // 4. Update Process
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProcess(Guid id, [FromBody] UpdateProcessDTO dto)
    {
        var userId = GetUserId();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _processService.UpdateProcessAsync(userId, id, dto);

        if (!result)
            return NotFound(new { Message = "Process not found" });

        return NoContent();
    }

    // 5. Delete Process
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProcess(Guid id)
    {
        var userId = GetUserId();
        var result = await _processService.DeleteProcessAsync(userId, id);

        if (!result)
            return NotFound(new { Message = "Process not found" });

        return NoContent();
    }
}
