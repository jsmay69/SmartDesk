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
    public class ReminderControllerIntegrationTests
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ReminderControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SendReminder_ReturnsOk_ForValidTodoItemDto()
        {
            // Arrange
            var dto = new TodoItemDto
            {
                Id = Guid.NewGuid(),
                Title = "Integration Test Task",
                Description = "Integration demo",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "Normal",
                IsCompleted = false
            };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/reminders/send", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
