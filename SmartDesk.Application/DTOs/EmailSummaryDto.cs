using System.Collections.Generic;

namespace SmartDesk.Application.DTOs
{
    public class ActionItemDto
    {
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response returned by the email summarization service.
    /// </summary>
    public class EmailSummaryDto
    {    /// <summary>
         /// The extracted subject of the email content.
         /// </summary>
        public string Subject { get; set; } = string.Empty;
        /// <summary>
        /// A brief natural-language summary of the email content.
        /// </summary>
        public string SummaryText { get; set; } = string.Empty;

        /// <summary>
        /// List of action items extracted from the email.
        /// </summary>
        public List<ActionItemDto> ActionItems { get; set; } = new List<ActionItemDto>();
    }
}
/// <summary>
/// Response returned by the email summarization service.
/// </summary>
public class EmailSummaryResponse
{
    /// <summary>
    /// A brief natural-language summary of the email content.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// List of action items extracted from the email.
    /// </summary>
    public List<string> ActionItems { get; set; }
}

