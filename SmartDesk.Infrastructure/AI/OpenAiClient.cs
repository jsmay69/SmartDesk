using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class OpenAiClient : ILLMClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;

    public OpenAiClient(IHttpClientFactory factory, IOptions<LlmSettings> options)
    {
        _httpClient = factory.CreateClient();
        _settings = options.Value.OpenAI;
    }

    public async Task<QueryIntentResult> GetIntentAsync(string prompt)
    {
        var systemMessage = new
        {
            role = "system",
            content = "Classify the user's prompt into a structured JSON object with the following format:\n" +
                      "{ \"intent\": \"<intent_code>\", \"priority\": \"<optional>\", \"timeframe\": \"<optional>\" }\n" +
                      "Valid intents: GetTasksDueToday, GetTasksDueTomorrow, GetTasksByPriority, GetOverdueTasks, GetFreeTime, Unknown.\n" +
                      "Respond with only the JSON object. No explanation, no formatting."
        };

        var userMessage = new { role = "user", content = prompt };

        var requestBody = new
        {
            model = "gpt-4",
            messages = new[] { systemMessage, userMessage }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        using var jsonDoc = JsonDocument.Parse(responseBody);
        var raw = jsonDoc.RootElement
                         .GetProperty("choices")[0]
                         .GetProperty("message")
                         .GetProperty("content")
                         .GetString()
                         ?.Trim();

        if (string.IsNullOrWhiteSpace(raw))
            return new QueryIntentResult { Intent = "Unknown" };

        try
        {
            var result = JsonSerializer.Deserialize<QueryIntentResult>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new QueryIntentResult { Intent = "Unknown" };
        }
        catch
        {
            return new QueryIntentResult { Intent = "Unknown" };
        }
    }
}