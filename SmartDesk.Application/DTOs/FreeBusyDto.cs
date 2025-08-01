using System;
using System.Collections.Generic;
using System.Data;
using SmartDesk.Application.DTOs;

namespace SmartDesk.Application.DTOs
{
  
    /// <summary>
    /// Response returned from the Calendar Planner agent with busy and free time slots.
    /// </summary>
    public class FreeBusyDto
    {
        /// <summary>
        /// The calendar ID that was queried.
        /// </summary>
        public string CalendarId { get; set; }

        /// <summary>
        /// List of time slots when the calendar is busy.
        /// </summary>
        public List<TimeSlotDto> BusySlots { get; set; }

        /// <summary>
        /// List of time slots when the calendar is free.
        /// </summary>
        public List<TimeSlotDto> FreeSlots { get; set; }
    }
}
