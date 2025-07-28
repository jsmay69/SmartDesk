using SmartDesk.Application.DTOs;
using System.Threading.Tasks;

namespace SmartDesk.Application.Interfaces;

public interface INaturalLanguageTaskParser
{
    Task<TodoItemDto> ParseAsync(string rawText);
}
