using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;

public class OllamaOrchestrationClient : IOrchestrationIntentClient
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;

    public OllamaOrchestrationClient(IHttpClientFactory factory, IOptions<LlmSettings> options)
    {
        _httpClient = factory.CreateClient();
        _settings = options.Value.Ollama;
        _httpClient.BaseAddress = new Uri(_settings.Endpoint);
    }

    public async Task<OrchestrationIntentResult> GetOrchestrationIntentAsync(string prompt)
    {
        var systemPrompt = "Classify the following user prompt into an orchestration task.\n" +
            "Valid values for \"intent\" include:\n" +
            "- PlanMyDay\n"+
            "- PlanMyWeek\n"+
            "- Unknown\n" +
            "Valid values for \"agents\" include:\n" +
            "= Calendar\n"+
            "- Email\n"+
            "- Query\n\n" +
            "Respond ONLY in this exact format:\n" +
            "{\n  \"intent\": \"<intent_code>\",\n  \"agents\": [\"Tasks\", \"Calendar\"]\n}\n\n" +
            $"User input: {prompt}";

        
        var body = new
        {
            model = _settings.Model,
            prompt = systemPrompt,
            stream = false
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/generate", content);
        var responseText = await response.Content.ReadAsStringAsync();

        using var jsonDoc = JsonDocument.Parse(responseText);
        var raw = jsonDoc.RootElement.GetProperty("response").GetString()?.Trim();

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