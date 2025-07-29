using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SmartDesk.Application.DTOs;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SmartDesk.API.Tests
{
    public class EmailControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EmailControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        //[Fact]
        public async Task Summarize_ReturnsSummary_ForJsonWrappedDto()
        {
            // Arrange
            var requestDto = new EmailSummarizeRequest
            {
                RawEmailText = "Subject: Test\n\nBody text.\n- Item1"
            };
            var json = JsonSerializer.Serialize(requestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/email/summarize", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var summary = JsonSerializer.Deserialize<EmailSummaryDto>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            summary.Should().NotBeNull();
            summary.Subject.Should().Contain("Subject: Test");
            summary.ActionItems.Should().HaveCount(1)
                   .And.ContainEquivalentOf(new ActionItemDto { Description = "Item1" });
        }
    }
}
