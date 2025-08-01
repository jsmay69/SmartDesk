/// <summary>


namespace SmartDesk.Application.DTOs
{/// Represents a time interval with a start and end.
 /// </summary>
    public class TimeSlotDto
    {
        /// <summary>
        /// Start time in UTC.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// End time in UTC.
        /// </summary>
        public DateTime End { get; set; }
    }
}
