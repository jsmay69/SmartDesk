using Microsoft.AspNetCore.Mvc;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;

namespace SmartDesk.API.Controllers;

[ApiController]
[Route("api/v1.0/orchestrate")]
public class OrchestratorController : ControllerBase
{
    private readonly IAgentOrchestrator _orchestrator;

    public OrchestratorController(IAgentOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    /// <summary>
    /// Processes a high-level user prompt by routing it through multiple agents (tasks, calendar, etc.).
    /// </summary>
    /// <param name="request">The user prompt, e.g., "Plan my day"</param>
    /// <returns>Unified structured response from all relevant agents</returns>
    /// <response code="200">Returns the orchestrated response</response>
    /// <response code="400">If the prompt is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrchestratedResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Orchestrate([FromBody] QueryRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt is required.");

        var result = await _orchestrator.HandleAsync(request.Prompt);
        return Ok(result);
    }
}
