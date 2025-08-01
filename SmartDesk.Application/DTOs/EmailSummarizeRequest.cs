namespace SmartDesk.Application.DTOs
{
    /// <summary>
    /// Request payload for summarizing an email message.
    /// </summary>
    public class EmailSummarizeRequest
    {
        /// <summary>
        /// The raw text of the email to summarize.
        /// </summary>
        public string RawEmailText { get; set; } = string.Empty;
    }
}