using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SmartDesk.Application.DTOs;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SmartDesk.API.Tests
{
    public class CalendarControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CalendarControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

       // [Fact]
        public async Task GetFreeBusy_ReturnsCorrectSlots()
        {
            // Arrange
            var request = new CalendarFreeBusyRequest
            {
                CalendarId = "primary",
                From = new DateTime(2025, 8, 1, 9, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2025, 8, 1, 17, 0, 0, DateTimeKind.Utc)
            };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/calendar/freebusy", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = JsonSerializer.Deserialize<FreeBusyDto>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            result.Should().NotBeNull();
            result.CalendarId.Should().Be("primary");
            result.BusySlots.Should().HaveCount(1);
            result.FreeSlots.Should().HaveCount(2);
        }
    }
}
