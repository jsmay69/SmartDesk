using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class OpenAiOrchestrationClient : IOrchestrationIntentClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;

    public OpenAiOrchestrationClient(IHttpClientFactory factory, IOptions<LlmSettings> options)
    {
        _httpClient = factory.CreateClient();
        _settings = options.Value.OpenAI;
        _httpClient.BaseAddress = new Uri(_settings.Endpoint);
    }


    public async Task<OrchestrationIntentResult> GetOrchestrationIntentAsync(string prompt)
    {
        var systemMessage = new
        {
            role = "system",
            content = "Classify the user's prompt into an orchestration task.\n" +
                      "Respond in this JSON format:\n" +
                      "{ \"intent\": \"PlanMyDay\", \"agents\": [\"Tasks\", \"Calendar\"] }\n" +
                      "Valid intents: PlanMyDay, PlanMyWeek, Unknown.\n" +
                      "Valid agents: Tasks, Calendar, Email, Query.\n" +
                      "Return only the JSON object. No explanation or formatting."
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
            return new OrchestrationIntentResult { Intent = "Unknown" };

        try
        {
            var result = JsonSerializer.Deserialize<OrchestrationIntentResult>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new OrchestrationIntentResult { Intent = "Unknown" };
        }
        catch
        {
            return new OrchestrationIntentResult { Intent = "Unknown" };
        }
    }
}