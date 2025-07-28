using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.AI
{
    public class TaskParserService : INaturalLanguageTaskParser
    {
        public Task<TodoItemDto> ParseAsync(string rawText)
        {
            var now = DateTime.Now;
            var title = rawText.Length > 50
                ? rawText.Substring(0, 50)
                : rawText;

            // Determine due date based on keywords
            DateTime dueDate = ParseDueDate(rawText.ToLowerInvariant(), now);

            var result = new TodoItemDto
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = $"Parsed from input: {rawText}",
                DueDate = dueDate,
                Priority = "Normal",
                IsCompleted = false
            };

            return Task.FromResult(result);
        }

        private DateTime ParseDueDate(string text, DateTime now)
        {
            // "this afternoon" ? today @ 3?PM
            if (text.Contains("this afternoon"))
                return now.Date.AddHours(15);

            // "this morning" ? today @ 9?AM
            if (text.Contains("this morning"))
                return now.Date.AddHours(9);

            // "tomorrow" variants
            if (text.Contains("tomorrow"))
            {
                var baseDate = now.Date.AddDays(1);
                if (text.Contains("tomorrow afternoon"))
                    return baseDate.AddHours(15);
                if (text.Contains("tomorrow morning"))
                    return baseDate.AddHours(9);
                // default tomorrow @ 9?AM
                return baseDate.AddHours(9);
            }

            // Explicit time: "at 3pm" or "at 15:30"
            var match = Regex.Match(text, @"\bat\s+(\d{1,2})(?::(\d{2}))?\s*(am|pm)?");
            if (match.Success)
            {
                int hour = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                int minute = match.Groups[2].Success
                    ? int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture)
                    : 0;
                var ampm = match.Groups[3].Value;
                if (!string.IsNullOrEmpty(ampm))
                {
                    if (ampm == "pm" && hour < 12) hour += 12;
                    if (ampm == "am" && hour == 12) hour = 0;
                }
                var candidate = now.Date.AddHours(hour).AddMinutes(minute);
                if (candidate <= now)
                    candidate = candidate.AddDays(1);
                return candidate;
            }

            // Default fallback: tomorrow @ 9?AM
            return now.Date.AddDays(1).AddHours(9);
        }
    }
}
