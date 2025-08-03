namespace SmartDesk.Application.DTOs
{
    public class QueryIntentResult
    {
        public string Intent { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;       // Optional
        public string Timeframe { get; set; } = string.Empty;         // Optional (e.g. "today", "this week")
    }
}
