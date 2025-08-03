/// <summary>
/// Represents a natural language query from the user.
/// </summary>
public class QueryRequestDto
{
    /// <summary>
    /// A user input string like "What's due tomorrow?" or "When am I free next week?"
    /// </summary>
    public string Prompt { get; set; } = string.Empty;
}
