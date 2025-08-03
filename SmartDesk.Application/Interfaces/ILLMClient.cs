using SmartDesk.Application.DTOs;

public interface ILLMClient
{
    Task<QueryIntentResult> GetIntentAsync(string prompt);
}