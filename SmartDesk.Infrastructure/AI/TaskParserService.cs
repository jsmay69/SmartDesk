using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.AI;

public class TaskParserService : INaturalLanguageTaskParser
{
    public Task<TodoItemDto> ParseAsync(string rawText)
    {
        var result = new TodoItemDto
        {
            Id = Guid.NewGuid(),
            Title = rawText.Length > 50 ? rawText[..50] : rawText,
            Description = $"Parsed from input: {rawText}",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = "Normal",
            IsCompleted = false
        };
        return Task.FromResult(result);
    }
}
