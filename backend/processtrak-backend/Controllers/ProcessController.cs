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
    private readonly ISchedulingService _schedulingService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProcessesController(
        IProcessService processService,
        IHttpContextAccessor httpContextAccessor,
        ISchedulingService schedulingService
    )
    {
        _processService = processService;
        _httpContextAccessor = httpContextAccessor;
        _schedulingService = schedulingService;
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

    [HttpPost("schedule/run")]
    public async Task<IActionResult> RunSchedule([FromBody] RunScheduleDTO dto)
    {
        var userId = GetUserId();
        var result = await _schedulingService.RunScheduleAsync(
            userId,
            dto.ProcessIds,
            dto.AlgorithmIds,
            dto.TimeQuantum.GetValueOrDefault()
        );

        return Ok(result);
    }

    [HttpGet("runs/get-all")]
    public async Task<IActionResult> GetAllSchedulesByUserId()
    {
        var userId = GetUserId();
        var runs = await _schedulingService.GetAllSchedulesByUserIdAsync(userId);

        if (runs == null)
            return NotFound(new { Message = "Schedules not found" });

        return Ok(runs);
    }

    [HttpDelete("runs/{id:guid}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        var userId = GetUserId();
        // Call the service to delete the schedule
        bool isDeleted = await _schedulingService.DeleteScheduleAsync(id, userId);

        // Return appropriate response based on deletion result
        if (isDeleted)
        {
            return NoContent(); // 204 No Content indicates successful deletion
        }
        else
        {
            return NotFound(new { Message = "Schedule not found or does not belong to the user." }); // Optional message
        }
    }

    [HttpGet("runs/{id:guid}/stats")]
    public async Task<IActionResult> GetStats(Guid id)
    {
        var userId = GetUserId();
        var run = await _schedulingService.GetScheduleById(id, userId);

        if (run == null)
            return NotFound(new { Message = "Schedule run not found" });

        return Ok(
            new
            {
                run.startTime,
                run.endTime,
                run.totalExecutionTime,
                run.averageWaitingTime,
                run.averageTurnaroundTime,
                run.AlgorithmsJson,
                run.ProcessesJson,
                Algorithms = run.algorithms.Select(a => a.name),
                Processes = run.processes.Select(p => new
                {
                    p.name,
                    p.waitingTime,
                    p.turnaroundTime,
                    p.completionTime,
                }),
            }
        );
    }
}
