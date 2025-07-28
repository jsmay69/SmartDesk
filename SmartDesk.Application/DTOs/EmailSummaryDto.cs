using System.Collections.Generic;

namespace SmartDesk.Application.DTOs
{
    public class ActionItemDto
    {
        public string Description { get; set; } = string.Empty;
    }

    public class EmailSummaryDto
    {
        public string Subject { get; set; } = string.Empty;
        public string SummaryText { get; set; } = string.Empty;
        public List<ActionItemDto> ActionItems { get; set; } = new List<ActionItemDto>();
    }
}
