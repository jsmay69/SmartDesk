using SmartDesk.Application.DTOs;

namespace SmartDesk.Application.Interfaces
{
    /// <summary>
    /// Parses free?form text into a strongly typed TodoItemDto.
    /// </summary>
    public interface INaturalLanguageTaskParser
    {
        Task<TodoItemDto> ParseAsync(string rawText);
    }
}