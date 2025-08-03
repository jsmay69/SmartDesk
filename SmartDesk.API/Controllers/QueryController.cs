using Microsoft.AspNetCore.Mvc;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;

namespace SmartDesk.API.Controllers;

[ApiController]
[Route("api/v1.0/query")]
public class QueryController : ControllerBase
{
    private readonly ILLMClient _llmClient;

    public QueryController(ILLMClient llmClient)
    {
        _llmClient = llmClient;
    }

    /// <summary>
    /// Process a natural language query and return tasks or schedule insights.
    /// </summary>
    /// <param name="request">Natural language query (e.g., "What’s due today?")</param>
    /// <returns>Query summary and structured list of results</returns>
    /// <response code="200">Returns a result with matched tasks or calendar info</response>
    /// <response code="400">If input is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(QueryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Query([FromBody] QueryRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt is required.");

        var result = await _llmClient.GetIntentAsync(request.Prompt);// _queryAgent.ProcessQueryAsync(request);
        return Ok(result);
    }
}
