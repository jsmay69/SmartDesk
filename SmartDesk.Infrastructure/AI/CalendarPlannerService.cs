using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;

namespace SmartDesk.Infrastructure.AI
{
    public class CalendarPlannerService : ICalendarPlannerService
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<FreeBusyDto> GetFreeBusyAsync(CalendarFreeBusyRequest req)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
