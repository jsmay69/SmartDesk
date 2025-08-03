using SmartDesk.Application.DTOs;
using SmartDesk.Domain.Entities;

namespace SmartDesk.Application.Extensions
{
    public static class TodoItemExtensions
    {
        public static ScheduleItemDto ToScheduleItemDto(this TodoItem task)
        {
            return new ScheduleItemDto
            {
                Title = task.Title,
                Start = task.DueDate ?? DateTime.MinValue,
                End = task.DueDate, // Optional, can be null
                IsTask = true,
                Priority = task.Priority
            };
        }
    }
}
