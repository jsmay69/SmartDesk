using SmartDesk.Application.DTOs;

/// <summary>
/// Represents the structured response to a natural language query.
/// </summary>
public class QueryResponseDto
{
    /// <summary>
    /// Natural language summary for display.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Optional list of actionable or scheduled items.
    /// </summary>
    public List<ScheduleItemDto> Items { get; set; } = new();
}
