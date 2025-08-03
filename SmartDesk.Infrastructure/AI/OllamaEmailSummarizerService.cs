using Microsoft.Extensions.Options;
using SmartDesk.Application.Configurations;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.AI
{
    public class OllamaEmailSummarizerService : IEmailSummarizerService
    {
        private readonly HttpClient _http;
        private readonly string _model;

        public OllamaEmailSummarizerService(
            HttpClient http,
            IOptions<LlmSettings> opts
        )
        {
            _http = http;
            _model = opts.Value.Ollama.Model;
        }

        public async Task<EmailSummaryDto> SummarizeAsync(string rawEmailText)
        {
            var systemInstruction = @"You are an email summarizer. Return ONLY JSON with:
```
{
  ""subject"": string,
  ""summaryText"": string,
  ""actionItems"": [ { ""description"": string } ]
}
```

EXAMPLE 1
Input:
Subject: Team Sync

Hi all,

We meet tomorrow.

- Prepare slides
- Send calendar invite

Output:
{
  ""subject"":""Team Sync"",
  ""summaryText"":""Hi all, We meet tomorrow."",
  ""actionItems"":[
    {""description"":""Prepare slides""},
    {""description"":""Send calendar invite""}
  ]
}

                EXAMPLE 2
Input:
                Hello Team,

                Please review the report and let me know any blockers.
Set up a follow-up call.

Output:
{
  ""subject"":""Hello Team,"",
  ""summaryText"":""Please review the report and let me know any blockers."",
  ""actionItems"":[
    {""description"":""review the report""},
    {""description"":""let me know any blockers""},
    {""description"":""Set up a follow-up call""}
  ]
}

                            Now summarize this email:
                            ";

            var prompt = systemInstruction + rawEmailText;

                            var payload = new { model = _model, prompt };

                            var response = await _http.PostAsJsonAsync("/v1/completions", payload);
                            response.EnsureSuccessStatusCode();

                            var rawJson = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("[Ollama Raw Response] " + rawJson);

                            var jsonResult = ExtractJson(rawJson);

                            var summary = JsonSerializer.Deserialize<EmailSummaryDto>(
                                jsonResult,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            ) ?? new EmailSummaryDto();

                            if (summary.ActionItems == null || !summary.ActionItems.Any())
                                summary.ActionItems = ExtractActionItems(rawEmailText);

                            if (string.IsNullOrWhiteSpace(summary.SummaryText))
                                summary.SummaryText = ExtractSummaryText(rawEmailText);

                            return summary;
                        }

        private static string ExtractJson(string rawJson)
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            if (root.TryGetProperty("completion", out var comp))
                return comp.GetString()!;
            if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var first = choices[0];
                if (first.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var content))
                    return content.GetString()!;
                if (first.TryGetProperty("text", out var text))
                    return text.GetString()!;
            }
            return rawJson;
        }

        private static List<ActionItemDto> ExtractActionItems(string rawText)
        {
            var items = new List<ActionItemDto>();
            var lines = rawText.Split('\n');
            var bulletPattern = new Regex("^(?:[-*]|\\d+\\.)\\s*(.+)$");
            foreach (var line in lines)
            {
                var match = bulletPattern.Match(line.Trim());
                if (match.Success)
                    items.Add(new ActionItemDto { Description = match.Groups[1].Value.Trim() });
            }
            // Imperative fallback: sentences starting with capital verb
            var sentPattern = new Regex("(?m)^[A-Z][a-z]+.*?[\\.!?]");
            foreach (Match m in sentPattern.Matches(rawText))
            {
                var sentence = m.Value.Trim();
                items.Add(new ActionItemDto { Description = sentence });
            }
            return items;
        }

        private static string ExtractSummaryText(string rawText)
        {
            var lines = rawText.Split('\n')
                .Where(l => !Regex.IsMatch(l.Trim(), "^(?:[-*]|\\d+\\.)\\s*"));
            return string.Join(" ", lines).Trim();
        }
    }
}
