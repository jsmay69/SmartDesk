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
        private readonly IOrchestrationIntentClient _intentClient;

        public AgentOrchestrator(
    IQueryAgentService queryAgent,
    ICalendarPlannerService calendarService,
    IOrchestrationIntentClient intentClient)
        {
            _queryAgent = queryAgent;
            _calendarService = calendarService;
            _intentClient = intentClient;
        }

        public async Task<OrchestratedResponseDto> HandleAsync(string prompt)
        {
            var intentResult = await _intentClient.GetOrchestrationIntentAsync(prompt);

            if (intentResult.Intent == "PlanMyDay")
            {
                var response = new OrchestratedResponseDto
                {
                    Summary = "Here’s your plan for the day:",
                    Items = new List<ScheduleItemDto>(),
                    Sources = new List<string>()
                };

                if (intentResult.Agents.Contains("Tasks"))
                {
                    var tasks = await _queryAgent.ProcessQueryAsync(new QueryRequestDto
                    {
                        Prompt = "What are my high priority tasks due today?"
                    });
                    response.Items.AddRange(tasks.Items);
                    response.Sources.Add("Tasks");
                }

                if (intentResult.Agents.Contains("Calendar"))
                {
                    var calendarRequest = new CalendarFreeBusyRequest
                    {
                        CalendarId = "primary",
                        From = DateTime.UtcNow.Date.AddHours(9),
                        To = DateTime.UtcNow.Date.AddHours(17)
                    };

                    var freeBusySlots = await _calendarService.GetFreeBusyAsync(calendarRequest);
                    
                    response.Items.AddRange(freeBusySlots.FreeSlots.Select(f => new ScheduleItemDto
                    {
                        Title = "Free Time",
                        Start = f.Start,
                        End = f.End,
                        IsTask = false
                    }));

                    response.Sources.Add("Calendar");
                }

                return response;
            }

            return new OrchestratedResponseDto
            {
                Summary = "Sorry, I don't yet know how to handle that.",
                Items = new List<ScheduleItemDto>()
            };
        }
    }

}
