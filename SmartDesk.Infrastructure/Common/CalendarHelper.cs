using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;           
using SmartDesk.Application.DTOs;            

namespace SmartDesk.Infrastructure.Common
{
    /// <summary>
    /// Helper to turn a list of busy TimePeriods into free TimeSlotDto spans.
    /// </summary>
    public static class CalendarHelper
    {
        public static List<TimeSlotDto> DeriveFreeSlots(
            DateTime windowStart,
            DateTime windowEnd,
            IList<TimePeriod> busyPeriods)
        {
            var free = new List<TimeSlotDto>();

            // sort the busy periods by start time
            var sorted = busyPeriods
                .Where(p => p.Start.HasValue && p.End.HasValue)
                .OrderBy(p => p.Start.Value)
                .ToList();

            var cursor = windowStart;

            foreach (var period in sorted)
            {
                var busyStart = period.Start.Value;
                var busyEnd = period.End.Value;

                // if there is a gap before this busy period, record it
                if (busyStart > cursor && cursor < windowEnd)
                {
                    free.Add(new TimeSlotDto
                    {
                       Start = cursor,
                       End = busyStart < windowEnd ? busyStart : windowEnd
                    });
                }

                // move the cursor forward if this busy period ends after it
                if (busyEnd > cursor)
                    cursor = busyEnd;
            }

            // finally, if there’s free time after the last busy period until windowEnd
            if (cursor < windowEnd)
            {
                free.Add(new TimeSlotDto { Start = cursor, End = windowEnd });
            }

            return free;
        }
    }
}
