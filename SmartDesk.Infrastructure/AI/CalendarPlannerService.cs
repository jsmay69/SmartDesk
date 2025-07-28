using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.AI
{
    public class CalendarPlannerService : ICalendarPlannerService
    {
        public Task<FreeBusyDto> GetFreeBusyAsync(string calendarId, DateTime from, DateTime to)
        {
            // Stub: one busy slot in the middle of the window,
            // free slots before and after.
            var spanHours = (to - from).TotalHours;
            var busyStart = from.AddHours(spanHours / 3);
            var busyEnd = busyStart.AddHours(1);

            var dto = new FreeBusyDto
            {
                CalendarId = calendarId,
                BusySlots = new()
                {
                    new TimeSlotDto { Start = busyStart, End = busyEnd }
                },
                FreeSlots = new()
                {
                    new TimeSlotDto { Start = from, End = busyStart },
                    new TimeSlotDto { Start = busyEnd, End = to }
                }
            };

            return Task.FromResult(dto);
        }
    }
}
