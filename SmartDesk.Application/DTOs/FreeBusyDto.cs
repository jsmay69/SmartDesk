using System;
using System.Collections.Generic;

namespace SmartDesk.Application.DTOs
{
    public class TimeSlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class FreeBusyDto
    {
        public string CalendarId { get; set; } = string.Empty;
        public List<TimeSlotDto> BusySlots { get; set; } = new();
        public List<TimeSlotDto> FreeSlots { get; set; } = new();
    }
}
