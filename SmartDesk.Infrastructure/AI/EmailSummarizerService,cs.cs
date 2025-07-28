using SmartDesk.Application.DTOs;
using SmartDesk.Application.Interfaces;
using System.Threading.Tasks;

namespace SmartDesk.Infrastructure.AI
{
    public class EmailSummarizerService : IEmailSummarizerService
    {
        public Task<EmailSummaryDto> SummarizeAsync(string rawEmailText)
        {
            // Simple stub: first line = subject, rest = summary; lines starting with '-' are action items
            var lines = rawEmailText.Split('\n');
            var subject = lines.Length > 0 ? lines[0].Trim() : string.Empty;
            var body = lines.Length > 1 ? string.Join(' ', lines, 1, lines.Length - 1).Trim() : string.Empty;

            var summary = new EmailSummaryDto
            {
                Subject = subject,
                SummaryText = body.Length > 200 ? body.Substring(0, 200) + "…" : body
            };

            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("-"))
                {
                    summary.ActionItems.Add(new ActionItemDto
                    {
                        Description = line.Trim().TrimStart('-').Trim()
                    });
                }
            }

            return Task.FromResult(summary);
        }
    }
}
