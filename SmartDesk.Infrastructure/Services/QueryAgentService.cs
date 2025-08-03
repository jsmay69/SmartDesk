using Google.Apis.Calendar.v3;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Extensions;
using SmartDesk.Application.Interfaces;
using SmartDesk.Domain.Entities;

public class QueryAgentService : IQueryAgentService
{
    private readonly ITodoItemRepository _taskRepository;
    private readonly ICalendarPlannerService _calendarService;
    private readonly ILLMClient _llm;

    public QueryAgentService(
        ITodoItemRepository taskRepository,
        ICalendarPlannerService calendarService,
        ILLMClient llm)
    {
        _taskRepository = taskRepository;
        _calendarService = calendarService;
        _llm = llm;
    }

    public async Task<QueryResponseDto> ProcessQueryAsync(QueryRequestDto request)
    {
        var intentResult = await _llm.GetIntentAsync(request.Prompt);
        var intent = intentResult.Intent?.ToLowerInvariant() ?? "unknown";
        var today = DateTime.UtcNow.Date;

        switch (intent)
        {
            case "gettasksduetoday":
                var todayTasks = await _taskRepository.FindAsync(t =>
                    t.DueDate.HasValue && t.DueDate.Value.Date == today && !t.IsCompleted);
                return CreateTaskResponse("Here are your tasks due today:", todayTasks);

            case "gettasksduetomorrow":
                var tomorrow = today.AddDays(1);
                var tomorrowTasks = await _taskRepository.FindAsync(t =>
                    t.DueDate.HasValue && t.DueDate.Value.Date == tomorrow && !t.IsCompleted);
                return CreateTaskResponse("Tasks due tomorrow:", tomorrowTasks);

            case "getoverduetasks":
                var overdueTasks = await _taskRepository.FindAsync(t =>
                    t.DueDate.HasValue && t.DueDate.Value.Date < today && !t.IsCompleted);
                return CreateTaskResponse("These tasks are overdue:", overdueTasks);

            case "gettasksbypriority":
                var priority = intentResult.Priority ?? "High";
                var priorityTasks = await _taskRepository.FindAsync(t =>
                    t.Priority == priority && !t.IsCompleted);
                return CreateTaskResponse($"Your {priority.ToLower()}-priority tasks:", priorityTasks);

            case "getfreetime":
                var calendarRequest = new CalendarFreeBusyRequest
                {
                    CalendarId = "primary",
                    From = today.AddHours(9),
                    To = today.AddHours(17)
                };

                var busy = await _calendarService.GetFreeBusyAsync(calendarRequest);
                var free = CalculateFreeSlots(calendarRequest, busy.FreeSlots);

                return new QueryResponseDto
                {
                    Summary = "Here’s when you're free today:",
                    Items = free.Select(f => new ScheduleItemDto
                    {
                        Title = "Free Time",
                        Start = f.Start,
                        End = f.End,
                        IsTask = false
                    }).ToList()
                };

            default:
                return new QueryResponseDto
                {
                    Summary = "Sorry, I couldn’t understand that.",
                    Items = new List<ScheduleItemDto>()
                };
        }
    }

    private static QueryResponseDto CreateTaskResponse(string summary, List<TodoItem> tasks)
    {
        return new QueryResponseDto
        {
            Summary = summary,
            Items = tasks.Select(t => t.ToScheduleItemDto()).ToList()
        };
    }

    private static List<TimeSlotDto> CalculateFreeSlots(CalendarFreeBusyRequest req, List<TimeSlotDto> busy)
    {
        var free = new List<TimeSlotDto>();
        var cursor = req.From;

        foreach (var b in busy.OrderBy(s => s.Start))
        {
            if (cursor < b.Start)
                free.Add(new TimeSlotDto { Start = cursor, End = b.Start });

            cursor = (cursor > b.End) ? cursor : b.End;
        }

        if (cursor < req.To)
            free.Add(new TimeSlotDto { Start = cursor, End = req.To });

        return free;
    }
}