using System.Threading.Tasks;
using FluentAssertions;
using SmartDesk.Application.Interfaces;
using SmartDesk.Infrastructure.AI;
using Xunit;

namespace SmartDesk.Application.Tests
{
    public class EmailSummarizerServiceTests
    {
        private readonly IEmailSummarizerService _service;

        public EmailSummarizerServiceTests()
        {
            _service = new EmailSummarizerService();
        }

       // [Fact]
        public async Task SummarizeAsync_ShouldExtractSubjectAndBodyAndActionItems()
        {
            // Arrange
            var rawEmail = @"Subject: Meeting Reminder

Hello team,

Please join the meeting tomorrow at 10 AM.

- Prepare slides
- Send agenda

Thanks,
Admin";

            // Act
            var result = await _service.SummarizeAsync(rawEmail);

            // Assert
            result.Subject.Should().Be("Subject: Meeting Reminder");
            result.SummaryText.Should().Contain("Hello team").And.Contain("Please join the meeting");
            result.ActionItems.Should().HaveCount(2);
            result.ActionItems[0].Description.Should().Be("Prepare slides");
            result.ActionItems[1].Description.Should().Be("Send agenda");
        }
    }
}
