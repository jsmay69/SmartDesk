using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using System.Text;
using System.Text.Json;

public class OllamaClient : ILLMClient
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;

    public OllamaClient(IHttpClientFactory factory, IOptions<LlmSettings> options)
    {
        _httpClient = factory.CreateClient();
        _settings = options.Value.Ollama;
        _httpClient.BaseAddress = new Uri(_settings.Endpoint);
    }

    public async Task<QueryIntentResult> GetIntentAsync(string prompt)
    {
        {
            var body = new
            {
                model = _settings.Model,
                Stream = false,
                prompt = prompt = $$$"""
Classify the following user request into one of the following intent categories:
- GetTasksDueToday
- GetTasksDueTomorrow
- GetOverdueTasks
- GetTasksByPriority
- GetFreeTime
- Unknown

Respond in this format:
{{
  "intent": "<intent_code>",
  "priority": "<optional>",
  "timeframe": "<optional>"
}}

User input: {prompt}

Respond ONLY with one of the intent codes above.
"""
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/generate", content);
            var resultJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(resultJson);
            var raw = jsonDoc.RootElement.GetProperty("response").GetString()?.Trim();

            if (string.IsNullOrWhiteSpace(raw))
                return new QueryIntentResult { Intent = "Unknown" };

            try
            {
                var intentResult = JsonSerializer.Deserialize<QueryIntentResult>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return intentResult ?? new QueryIntentResult { Intent = "Unknown" };
            }
            catch
            {
                return new QueryIntentResult { Intent = "Unknown" };
            }
        }
    }
}