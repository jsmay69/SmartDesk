using Microsoft.Extensions.Options;
using Prometheus;
using SmartDesk.Application.Configuration;
using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;




namespace SmartDesk.Infrastructure.AI
{
    /// <summary>
    /// Uses OpenAI's chat completions API to summarize emails into structured JSON,
    /// with deterministic fallbacks for bullet and imperative extraction.
    /// </summary>
    public class OpenAIEmailSummarizerService : IEmailSummarizerService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;

        private static readonly Counter _emailSummaries = Metrics
    .CreateCounter("smartdesk_email_summaries_total",
                   "Total number of email summarizations performed");
        public OpenAIEmailSummarizerService(
            HttpClient http,
            IOptions<EmailSummarizerSettings> opts)
        {
            _http = http;
            _apiKey = opts.Value.OpenAIApiKey;
            _model = opts.Value.OpenAIModel;
        }

        public async Task<EmailSummaryDto> SummarizeAsync(string rawEmailText)
        {
            // Prepare system prompt with examples
            var systemInstruction = @"
You are an email summarizer. Return ONLY JSON with:
{
  'subject': string,
  'summaryText': string,
  'actionItems': [ { 'description': string } ]
}
Do not include any commentary, only the JSON object.

EXAMPLE 1
Input:
Subject: Team Sync

Hi all,

We meet tomorrow.

- Prepare slides
- Send calendar invite

Output:
{
  'subject':'Team Sync',
  'summaryText':'Hi all, We meet tomorrow.',
  'actionItems':[
    {'description':'Prepare slides'},
    {'description':'Send calendar invite'}
  ]
}

EXAMPLE 2
Input:
Hello Team,

Please review the report and let me know any blockers.
Set up a follow-up call.

Output:
{
  'subject':'Hello Team,',
  'summaryText':'Please review the report and let me know any blockers.',
  'actionItems':[
    {'description':'review the report'},
    {'description':'let me know any blockers'},
    {'description':'Set up a follow-up call'}
  ]
}

Now summarize this email:
";

            var userMessage = rawEmailText;

            // Build the chat completion payload
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system",  content = systemInstruction },
                    new { role = "user",    content = userMessage      }
                },
                temperature = 0.3
            };

            // Authorize and call OpenAI
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var response = await _http.PostAsJsonAsync("/v1/chat/completions", payload);
            response.EnsureSuccessStatusCode();

            // Read raw response
            var rawJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine("[OpenAI Raw Response] " + rawJson);

            // Extract JSON content
            var jsonResult = ExtractJson(rawJson);

            // Deserialize
            var summary = JsonSerializer.Deserialize<EmailSummaryDto>(
                jsonResult,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new EmailSummaryDto();

            // Fallback extraction
            if (summary.ActionItems == null || !summary.ActionItems.Any())
                summary.ActionItems = ExtractActionItems(rawEmailText);
            if (string.IsNullOrWhiteSpace(summary.SummaryText))
                summary.SummaryText = ExtractSummaryText(rawEmailText);

            _emailSummaries.Inc();
            return summary;
        }

        private static string ExtractJson(string rawJson)
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            // Chat API: choices[0].message.content
            if (root.TryGetProperty("choices", out var choices)
                && choices.ValueKind == JsonValueKind.Array
                && choices.GetArrayLength() > 0)
            {
                var first = choices[0];
                if (first.TryGetProperty("message", out var msg)
                    && msg.TryGetProperty("content", out var content))
                {
                    return content.GetString()!;
                }
            }
            // Fallback: plain completions endpoint
            if (root.TryGetProperty("completion", out var comp))
                return comp.GetString()!;

            // Return rawJson if no wrapper found
            return rawJson;
        }

        private static List<ActionItemDto> ExtractActionItems(string rawText)
        {
            var items = new List<ActionItemDto>();
            var lines = rawText.Split('\n');
            var bulletPattern = new Regex(@"^(?:[-*]|\d+\.)\s*(.+)$");
            foreach (var line in lines)
            {
                var match = bulletPattern.Match(line.Trim());
                if (match.Success)
                    items.Add(new ActionItemDto { Description = match.Groups[1].Value.Trim() });
            }
            // Imperative fallback
            var sentPattern = new Regex(@"(?m)(^[A-Z][^\.\!?]+[\.\!?])");
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
                .Where(l => !Regex.IsMatch(l.Trim(), @"^(?:[-*]|\d+\.)\s*"));
            return string.Join(" ", lines).Trim();
        }
    }
}
