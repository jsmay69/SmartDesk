using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;

namespace SmartDesk.Infrastructure.AI
{
    public class MicrosoftCalendarPlannerService : ICalendarPlannerService
    {
        private readonly GraphServiceClient _graph;

        public MicrosoftCalendarPlannerService(IOptions<CalendarSettings> opts)
        {
            var s = opts.Value;
            var credential = new ClientSecretCredential(
                s.MicrosoftTenantId,
                s.MicrosoftClientId,
                s.MicrosoftClientSecret
            );
            _graph = new GraphServiceClient(credential);
        }

        public async Task<FreeBusyDto> GetFreeBusyAsync(CalendarFreeBusyRequest req)
        {
            // 1) Build the request body
            var requestBody = new Microsoft.Graph.Users.Item.Calendar.GetSchedule.GetSchedulePostRequestBody
            {
                Schedules = new List<string> { req.CalendarId },
                StartTime = new DateTimeTimeZone
                {
                    DateTime = req.From.ToString("o"),
                    TimeZone = "UTC"
                },
                EndTime = new DateTimeTimeZone
                {
                    DateTime = req.To.ToString("o"),
                    TimeZone = "UTC"
                },
                AvailabilityViewInterval = 30
            };

            // 2) Invoke the function via the property builder
            var scheduleResponse = await _graph.Users[req.CalendarId]
                                               .Calendar
                                               .GetSchedule
                                               .PostAsync(requestBody);

            // 3) Flatten out all returned ScheduleItems
            var scheduleItems = scheduleResponse.Value?
                .SelectMany(r => r.ScheduleItems ?? new List<ScheduleItem>())
                .ToList()
                ?? new List<ScheduleItem>();

            // 4) Map to your DTOs
            var busySlots = scheduleItems
                .Select(i => new TimeSlotDto {
                   Start = DateTime.Parse(i.Start.DateTime),
                   End = DateTime.Parse(i.End.DateTime)})
                .ToList();

            // 5) Derive free slots between those busy intervals
            var freeSlots = DeriveFreeSlots(req.From, req.To, busySlots);

            return new FreeBusyDto
            {
                CalendarId = req.CalendarId,
                BusySlots = busySlots,
                FreeSlots = freeSlots
            };
        }

        // Helper: derive free slots from busy TimeSlotDto list
        private static List<TimeSlotDto> DeriveFreeSlots(
            DateTime windowStart,
            DateTime windowEnd,
            List<TimeSlotDto> busySlots)
        {
            var free = new List<TimeSlotDto>();
            var sorted = busySlots.OrderBy(b => b.Start).ToList();
            var cursor = windowStart;

            foreach (var slot in sorted)
            {
                if (slot.Start > cursor)
                    free.Add(new TimeSlotDto { Start = cursor, End = slot.Start });

                if (slot.End > cursor)
                    cursor = slot.End;
            }

            if (cursor < windowEnd)
                free.Add(new TimeSlotDto { Start = cursor, End = windowEnd });

            return free;
        }
    }
}
