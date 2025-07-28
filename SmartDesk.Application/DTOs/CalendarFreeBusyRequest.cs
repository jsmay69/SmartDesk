using System;

namespace SmartDesk.Application.DTOs
{
    public class CalendarFreeBusyRequest
    {
        public string CalendarId { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
