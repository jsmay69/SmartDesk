using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.AI
{
    public class CalendarPlannerService : ICalendarPlannerService
    {
        public async Task<FreeBusyDto> GetFreeBusyAsync(CalendarFreeBusyRequest req)
        {
            // Stub: one busy slot in the middle of the window,
            // free slots before and after.
            var spanHours = (req.To - req.From).TotalHours;
            var busyStart = req.From.AddHours(spanHours / 3);
            var busyEnd = busyStart.AddHours(1);

            return new FreeBusyDto
            {
                CalendarId = req.CalendarId,
                BusySlots = new()
                {
                    new TimeSlotDto { Start = busyStart, End = busyEnd }
                },
                FreeSlots = new()
                {
                    new TimeSlotDto { Start = req.From, End = busyStart },
                    new TimeSlotDto { Start = busyEnd, End = req.To }
                }
            };
        }
    }
}
