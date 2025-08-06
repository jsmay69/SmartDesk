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
            var systemPrompt = "Classify the following user input into a structured JSON object.\n" +
           "Valid values for \"intent\" include:\n" +
           "- GetTasksDueToday\n" +
           "- GetTasksDueTomorrow\n" +
           "- GetTasksByPriority\n" +
           "- GetOverdueTasks\n" +
           "- GetFreeTime\n" +
           "- Unknown\n\n" +
           "Respond ONLY in this exact format:\n" +
           "{\n  \"intent\": \"<intent_code>\",\n  \"priority\": \"<optional>\",\n  \"timeframe\": \"<optional>\"\n}\n\n" +
           $"User input: {prompt}";

            var body = new
            {
                model = _settings.Model,
                prompt = systemPrompt,
                stream = false
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