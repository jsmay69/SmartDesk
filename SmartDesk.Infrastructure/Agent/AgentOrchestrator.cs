using Google.Apis.Calendar.v3;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.Agent
{
    public class AgentOrchestrator : IAgentOrchestrator
    {
        private readonly IQueryAgentService _queryAgent;
        private readonly ICalendarPlannerService _calendarService;

        public AgentOrchestrator(
            IQueryAgentService queryAgent,
            ICalendarPlannerService calendarService)
        {
            _queryAgent = queryAgent;
            _calendarService = calendarService;
        }

        public async Task<OrchestratedResponseDto> HandleAsync(string prompt)
        {
            if (prompt.ToLower().Contains("plan my day"))
            {
                var taskQuery = new QueryRequestDto { Prompt = "What are my tasks due today?" };
                var taskResult = await _queryAgent.ProcessQueryAsync(taskQuery);

                var calendarRequest = new CalendarFreeBusyRequest
                {
                    CalendarId = "primary",
                    From = DateTime.UtcNow.Date.AddHours(9),
                    To = DateTime.UtcNow.Date.AddHours(17)
                };

                FreeBusyDto calendarResult = await _calendarService.GetFreeBusyAsync(calendarRequest);
              
                return new OrchestratedResponseDto
                {
                    Summary = "Here’s your plan for the day:",
                    Items = taskResult.Items
                        .Concat(calendarResult.FreeSlots.Select(slot => new ScheduleItemDto
                        {
                            Title = "Free Time",
                            Start = slot.Start,
                            End = slot.End,
                            IsTask = false
                        })).ToList(),
                    Sources = new[] { "Tasks", "Calendar" }
                };
            }

            return new OrchestratedResponseDto
            {
                Summary = "Sorry, I don't yet know how to handle that.",
                Items = new List<ScheduleItemDto>()
            };
        }
    }

}
