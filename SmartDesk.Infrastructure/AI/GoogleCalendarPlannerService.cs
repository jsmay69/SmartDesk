using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Linq;
using System;
using System.Threading.Tasks;
using SmartDesk.Infrastructure.Common;

namespace SmartDesk.Infrastructure.AI
{
    public class GoogleCalendarPlannerService : ICalendarPlannerService
    {
        private readonly CalendarService _calendar;

        public GoogleCalendarPlannerService(IOptions<CalendarSettings> opts)
        {
            var settings = opts.Value;
            // Load service account credential
            var credential = GoogleCredential
                .FromFile(settings.GoogleServiceAccountJsonPath)
                .CreateScoped(CalendarService.Scope.CalendarReadonly);

            _calendar = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "SmartDeskAgent"
            });
        }

        public async Task<FreeBusyDto> GetFreeBusyAsync(CalendarFreeBusyRequest req)
        {
    // Prepare the Google FreeBusy request
    var freeBusyReq = new FreeBusyRequest
    {
        TimeMin = req.From,
        TimeMax = req.To,
        Items   = new[] { new FreeBusyRequestItem { Id = req.CalendarId } }
    };

    // Execute against Google Calendar API
    var fb = await _calendar.Freebusy.Query(freeBusyReq).ExecuteAsync();

    // Grab the raw TimePeriod list
    var busyTimePeriods = fb.Calendars[req.CalendarId].Busy.ToList();

    // Project into your DTOs
    var busySlots = busyTimePeriods
        .Select(tp => new TimeSlotDto {Start = tp.Start.Value, End = tp.End.Value }).ToList();

    // Derive free slots from the original TimePeriods
    var freeSlots = CalendarHelper.DeriveFreeSlots(
        req.From,
        req.To,
        busyTimePeriods
    );

    return new FreeBusyDto
    {
        CalendarId = req.CalendarId,
        BusySlots  = busySlots,
        FreeSlots  = freeSlots
    };
}
    }
}
