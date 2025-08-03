/// <summary>
/// A unified representation of a task or calendar event.
/// </summary>
public class ScheduleItemDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; }   // For calendar events
    public DateTime? End { get; set; }
    public bool IsTask { get; set; }      // True = task, False = event
    public string Priority { get; set; } = string.Empty;  // Optional: High/Normal/Low
}
