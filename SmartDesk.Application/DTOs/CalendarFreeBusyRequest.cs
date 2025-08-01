using System;

namespace SmartDesk.Application.DTOs
{
    /// <summary>
    /// Represents a request to retrieve free/busy time slots from a calendar.
    /// </summary>
    public class CalendarFreeBusyRequest
    {
        /// <summary>
        /// The ID of the calendar (e.g., "primary" or a full email address).
        /// </summary>
        public string CalendarId { get; set; }

        /// <summary>
        /// The start of the time window to query (UTC).
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// The end of the time window to query (UTC).
        /// </summary>
        public DateTime To { get; set; }
    }
}
